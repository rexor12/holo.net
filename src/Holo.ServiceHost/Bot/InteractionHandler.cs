using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Holo.Sdk.DI;
using Holo.Sdk.Interactions;
using Holo.Sdk.Interactions.Attributes;
using Holo.Sdk.Localization;
using Holo.Sdk.Logging;
using Holo.Sdk.Modules;
using Holo.ServiceHost.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.ServiceHost.Bot;

/// <summary>
/// Entry point for Discord interactions.
/// </summary>
[Service(typeof(InteractionHandler))]
public sealed class InteractionHandler
{
    private IReadOnlyDictionary<ulong, ModuleInfo[]>? _guildBoundModules;

    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IOptions<DiscordOptions> _options;
    private readonly IEnumerable<ModuleDescriptor> _moduleDescriptors;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactionService,
        IOptions<DiscordOptions> options,
        ILocalizationService localizationService,
        ILogger<InteractionHandler> logger,
        IEnumerable<ModuleDescriptor> moduleDescriptors,
        IServiceProvider serviceProvider)
    {
        _client = client;
        _interactionService = interactionService;
        _options = options;
        _moduleDescriptors = moduleDescriptors;
        _localizationService = localizationService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Initializes the interaction handler.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the operation.</returns>
    public async Task InitializeAsync()
    {
        _client.Ready += ReadyAsync;
        _client.InteractionCreated += ExecuteInteractionAsync;
        // _client.Log += message => _logger.LogAsync(message);
        // _interactionService.Log += message => _logger.LogAsync(message);
        _interactionService.InteractionExecuted += OnInteractionExecutedAsync;

        var interactionModuleTypes = _moduleDescriptors.SelectMany(descriptor => descriptor.Assembly.GetInteractionGroups());
        foreach (var interactionModuleType in interactionModuleTypes)
            await _interactionService.AddModuleAsync(interactionModuleType, _serviceProvider);

        _guildBoundModules = GetGuildBoundModules();
    }

    private static string GetCommandId(IDiscordInteractionData interactionData)
        => interactionData switch
        {
            SocketSlashCommandData data => data.Name,
            SocketMessageComponentData data => data.CustomId,
            SocketMessageCommandData data => data.Name,
            SocketUserCommandData data => data.Name,
            SocketAutocompleteInteractionData data => data.CommandName,
            _ => "<unknown>"
        };

    private async Task ReadyAsync()
    {
        if (_options.Value.DevelopmentServerId > 0)
            await _interactionService.RegisterCommandsToGuildAsync(_options.Value.DevelopmentServerId, true);
        else
            await _interactionService.RegisterCommandsGloballyAsync(true);

        if (_guildBoundModules?.Count is null or 0)
            return;

        foreach (var (guildId, moduleInfos) in _guildBoundModules)
        {
            _logger.LogDebug("Registering {Count} guild specific commands to guild '{GuildId}'", moduleInfos.Length, guildId);
            await _interactionService.AddModulesToGuildAsync(guildId, false, moduleInfos);
            _logger.LogDebug("Successfully {Count} registered guild specific commands to guild '{GuildId}'", moduleInfos.Length, guildId);
        }
    }

    private IReadOnlyDictionary<ulong, ModuleInfo[]> GetGuildBoundModules()
    {
        var guildBoundModules = new Dictionary<ulong, List<ModuleInfo>>();
        var interactionGroups = _moduleDescriptors
            .Select(descriptor => descriptor.Assembly)
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass
                            && !type.IsAbstract
                            && type.IsAssignableTo(typeof(InteractionGroupBase)))
            .Select(type => (
                InteractionGroup: type,
                GuildBoundAttribute: type.GetCustomAttribute<GuildBoundAttribute>()))
            .Where(item => item.GuildBoundAttribute != null);
        foreach (var (interactionGroup, guildBoundAttribute) in interactionGroups)
        {
            var moduleInfo = GetModuleInfo(interactionGroup);
            foreach (var guildId in guildBoundAttribute!.GuildIds)
            {
                if (!guildBoundModules.TryGetValue(guildId, out var moduleInfos))
                    guildBoundModules[guildId] = moduleInfos = new List<ModuleInfo>();

                moduleInfos.Add(moduleInfo);
            }
        }

        return guildBoundModules.ToDictionary(i => i.Key, i => i.Value.ToArray());
    }

    private ModuleInfo GetModuleInfo(Type interactionGroupType)
    {
        var getModuleInfoMethod = typeof(InteractionService).GetMethod(
            nameof(InteractionService.GetModuleInfo),
            Array.Empty<Type>());
        if (getModuleInfoMethod == null)
            throw new NotSupportedException("The required 'GetModuleInfo' method cannot be found. The code may need to be updated.");

        var closedGenericMethod = getModuleInfoMethod.MakeGenericMethod(interactionGroupType);
        var result = closedGenericMethod.Invoke(_interactionService, null);
        if (result is not ModuleInfo moduleInfo)
            throw new NotSupportedException("Found the wrong 'GetModuleInfo' method. The code may need to be updated.");

        return moduleInfo;
    }

    private Task ExecuteInteractionAsync(SocketInteraction interaction)
    {
        if (interaction is SocketMessageComponent componentInteraction)
            return ExecuteComponentInteractionAsync(componentInteraction);

        return _interactionService.ExecuteCommandAsync(
            new ExtendedInteractionContext(_client, interaction),
            _serviceProvider);
    }

    private Task ExecuteComponentInteractionAsync(SocketMessageComponent interaction)
    {
        if (!ComponentHelper.TryParseCustomId(interaction.Data.CustomId, out var componentInfo))
        {
            return interaction.RespondAsync(
                _localizationService.Localize("Interactions.InvalidInteractionError"),
                ephemeral: true);
        }

        return _interactionService.ExecuteCommandAsync(
            new ExtendedInteractionContext(_client, interaction)
            {
                ComponentInfo = componentInfo
            },
            _serviceProvider);
    }

    private Task OnInteractionExecutedAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
        => TryHandleResultAsync(context.Interaction, result);

    private async Task<bool> TryHandleResultAsync(IDiscordInteraction interaction, IResult result)
    {
        if (result.IsSuccess)
            return true;

        switch (result)
        {
            case LocalizedPreconditionResult r:
                _logger.LogDebug(
                    "A precondition error occurred during the execution of the command '{CommandId}': {ErrorMessage}",
                    GetCommandId(interaction.Data),
                    result.ErrorReason);

                if (!interaction.HasResponded)
                    await interaction.RespondAsync(
                        _localizationService.Localize(r.LocalizationKey, r.LocalizationArguments),
                        ephemeral: true);

                return true;

            default:
                _logger.LogError(
                    "An unhandled error occurred during the execution of the command '{CommandId}': {ErrorMessage}",
                    GetCommandId(interaction.Data),
                    result.ErrorReason);

                if (!interaction.HasResponded)
                    await interaction.RespondAsync(
                        _localizationService.Localize("Interactions.UnhandledInteractionError"),
                        ephemeral: true);

                return true;
        }
    }
}