using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Holo.Sdk.Collections;
using Holo.Sdk.DI;
using Holo.Sdk.Lifecycle;
using Holo.Sdk.Localization;
using Holo.ServiceHost.Resources;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Holo.ServiceHost.Localization;

/// <summary>
/// A service used for localization.
/// </summary>
[Service(typeof(ILocalizationService), typeof(IStartable))]
public sealed class LocalizationService : ILocalizationService, IStartable
{
    private const string CultureCodeGroupName = "CultureCode";

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, LocalizedValueHolder>> EmptyLocalizations
        = new Dictionary<string, IReadOnlyDictionary<string, LocalizedValueHolder>>();
    private static readonly IReadOnlyDictionary<string, LocalizedValueHolder> EmptyLocalization
        = new Dictionary<string, LocalizedValueHolder>();

    private IReadOnlyDictionary<string, IReadOnlyDictionary<string, LocalizedValueHolder>> _localizations = EmptyLocalizations;
    private IReadOnlyDictionary<string, LocalizedValueHolder> _defaultLocalization = EmptyLocalization;
    private readonly ILogger<LocalizationService> _logger;
    private readonly IOptions<ResourceOptions> _options;

    /// <inheritdoc cref="IStartable.Priority"/>
    public int Priority => 1;

    public LocalizationService(
        ILogger<LocalizationService> logger,
        IOptions<ResourceOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc cref="IStartable.StartAsync(CancellationToken)"/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var localizationFiles = GetLocalizationFiles(_options.Value).ToArray();
        if (localizationFiles.Length == 0)
        {
            _logger.LogWarning("Could not find any localization files");
            return;
        }

        _localizations = await localizationFiles
            .GroupBy(descriptor => descriptor.CultureCode)
            .Select(async descriptor => (
                CultureCode: descriptor.Key,
                Items: await ParseLocalizationFileAsync(descriptor.First().FilePath)))
            .WhenAllAsync()
            .ToDictionaryAsync(tuple => tuple.CultureCode, tuple => tuple.Items);
        _defaultLocalization = _localizations.TryGetValue(_options.Value.DefaultCultureCode, out var defaultLocalization)
            ? defaultLocalization
            : EmptyLocalization;
    }

    /// <inheritdoc cref="IStartable.StopAsync"/>
    public Task StopAsync()
        => Task.CompletedTask;

    /// <inheritdoc cref="ILocalizationService.Localize(string, ValueTuple{string, object?}[]?)"/>
    public string Localize(string key, params (string Name, object? Value)[]? arguments)
        => InternalLocalize(key, 0, arguments);

    /// <inheritdoc cref="ILocalizationService.Localize(string, IEnumerable{ValueTuple{string, object?}})"/>
    public string Localize(string key, IEnumerable<(string Name, object? Value)> arguments)
        => InternalLocalize(key, 0, arguments);

    /// <inheritdoc cref="ILocalizationService.Localize(string, int, ValueTuple{string, object?}[]?)"/>
    public string Localize(string key, int itemIndex, params (string Name, object? Value)[]? arguments)
        => InternalLocalize(key, itemIndex, arguments);

    private static IEnumerable<FileDescriptor> GetLocalizationFiles(ResourceOptions options)
    {
        var rootDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        var matcher = new Matcher();
        matcher.AddInclude(options.LocalizationGlobPattern);
        var results = matcher.Execute(new DirectoryInfoWrapper(rootDirectory));
        if (!results.HasMatches)
            yield break;

        var fileNameRegex = new Regex(options.LocalizationFileNamePattern);
        if (!fileNameRegex.GetGroupNames().Any(groupName => groupName == CultureCodeGroupName))
            throw new ArgumentException(
                $"The option '{nameof(ResourceOptions.LocalizationFileNamePattern)}'"
                + " must have a capture group called '{CultureCodeGroupName}'.",
                nameof(options));

        foreach (var result in results.Files)
        {
            var filePath = Path.Combine(rootDirectory.FullName, result.Path);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var fileNameMatch = fileNameRegex.Match(fileName);
            if (!fileNameMatch.Success)
                continue;

            yield return new FileDescriptor(
                fileNameMatch.Groups[CultureCodeGroupName].Value,
                filePath);
        }
    }

    private static async Task<IReadOnlyDictionary<string, LocalizedValueHolder>> ParseLocalizationFileAsync(string filePath)
    {
        JObject jObject;
        await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        await using (var jsonReader = new JsonTextReader(streamReader))
            jObject = await JObject.LoadAsync(jsonReader);

        var localization = new Dictionary<string, LocalizedValueHolder>();
        ParseLocalizationRecursively(jObject, new List<string>(10), localization);

        return localization;
    }

    private static void ParseLocalizationRecursively(
        JObject root,
        List<string> pathSegments,
        Dictionary<string, LocalizedValueHolder> localization)
    {
        foreach (var jProperty in root.Children<JProperty>())
        {
            if (jProperty.Value is JArray jArray)
            {
                pathSegments.Add(jProperty.Name);
                var templateStrings = jProperty.Value
                    .Children<JValue>()
                    .Values<string>()
                    .Select(ParseTemplateString)
                    .ToArray();
                localization[string.Join(".", pathSegments)] = new TemplateStringArray(templateStrings);
                pathSegments.RemoveAt(pathSegments.Count - 1);
                continue;
            }

            if (jProperty.Value is JValue jValue)
            {
                pathSegments.Add(jProperty.Name);
                localization[string.Join(".", pathSegments)] = ParseTemplateString(jValue.Value?.ToString());
                pathSegments.RemoveAt(pathSegments.Count - 1);
                continue;
            }

            if (jProperty.Value is JObject jObject)
            {
                pathSegments.Add(jProperty.Name);
                ParseLocalizationRecursively(jObject, pathSegments, localization);
                pathSegments.RemoveAt(pathSegments.Count - 1);
            }
        }
    }

    private static TemplateString ParseTemplateString(string? format)
    {
        if (string.IsNullOrEmpty(format))
            return TemplateString.Empty;

        return TemplateString.FromNamedFormatString(format);
    }

    private string InternalLocalize(string key, int itemIndex, IEnumerable<(string Name, object? Value)>? arguments)
    {
        // TODO Ambient request context -> culture code.
        if (!_defaultLocalization.TryGetValue(key, out var valueHolder))
            return key;

        return valueHolder switch
        {
            TemplateString templateString => templateString.Format(arguments),
            TemplateStringArray templateStringArray => templateStringArray.Format(itemIndex, arguments) ?? key,
            _ => key
        };
    }

    private readonly record struct FileDescriptor(string CultureCode, string FilePath);

    private abstract class LocalizedValueHolder
    {
        public abstract object RawValue { get; }
    }

    private abstract class LocalizedValueHolder<T> : LocalizedValueHolder
        where T : notnull
    {
        public override object RawValue => Value;

        public T Value { get; }

        public LocalizedValueHolder(T value)
        {
            Value = value;
        }
    }

    private sealed class TemplateString : LocalizedValueHolder<string>
    {
        private static readonly IReadOnlyDictionary<string, int> EmptyArgumentIndexes = new Dictionary<string, int>();
        private static readonly Regex FormatStringRegex = new Regex(@"{(?<Name>\w+)(?<Format>:(?:[^}]*?))?}", RegexOptions.Compiled);

        public static readonly TemplateString Empty = new TemplateString(string.Empty, EmptyArgumentIndexes);

        public IReadOnlyDictionary<string, int> ArgumentIndexes { get; }

        public TemplateString(string template, IReadOnlyDictionary<string, int>? argumentIndexes = null)
            : base(template)
        {
            ArgumentIndexes = argumentIndexes ?? EmptyArgumentIndexes;
        }

        public static TemplateString FromNamedFormatString(string formatString)
        {
            var argumentIndexes = new Dictionary<string, int>();
            var currentIndex = 0;
            var template = FormatStringRegex.Replace(formatString, m =>
            {
                argumentIndexes[m.Groups["Name"].Value] = currentIndex;
                return $"{{{currentIndex++}{m.Groups["Format"].Value}}}";
            });

            return new TemplateString(template, argumentIndexes);
        }

        public string Format(IEnumerable<(string Name, object? Value)>? arguments)
        {
            if (arguments == null)
                return Value;

            var indexedArguments = new object?[ArgumentIndexes.Count];
            foreach (var argument in arguments)
            {
                if (!ArgumentIndexes.TryGetValue(argument.Name, out var argumentIndex))
                    continue;

                indexedArguments[argumentIndex] = argument.Value;
            }

            return string.Format(Value, indexedArguments);
        }
    }

    private sealed class TemplateStringArray : LocalizedValueHolder<IReadOnlyList<TemplateString>>
    {
        public TemplateStringArray(IReadOnlyList<TemplateString> templateStrings)
            : base(templateStrings)
        {
        }

        public string? Format(int itemIndex, IEnumerable<(string Name, object? Value)>? arguments)
        {
            if (itemIndex < 0 || itemIndex >= Value.Count)
                return null;

            return Value[itemIndex].Format(arguments);
        }
    }
}