using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Holo.Sdk.DI;
using Holo.Sdk.Interactions;
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
        IEnumerable<ModuleDescriptor> moduleDescriptors,
        ILocalizationService localizationService,
        ILogger<InteractionHandler> logger,
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
        _interactionService.Log += message => _logger.LogAsync(message);
        _interactionService.SlashCommandExecuted += OnSlashCommandExecutedAsync;

        var interactionModuleTypes = _moduleDescriptors.SelectMany(descriptor => descriptor.Assembly.GetInteractionGroups());
        foreach (var interactionModuleType in interactionModuleTypes)
            await _interactionService.AddModuleAsync(interactionModuleType, _serviceProvider);

        _client.InteractionCreated += interaction => _interactionService.ExecuteCommandAsync(
            new SocketInteractionContext(_client, interaction),
            _serviceProvider);
    }

    private async Task ReadyAsync()
    {
        if (_options.Value.DevelopmentServerId > 0)
            await _interactionService.RegisterCommandsToGuildAsync(_options.Value.DevelopmentServerId, true);
        else
            await _interactionService.RegisterCommandsGloballyAsync(true);
    }

    private Task OnSlashCommandExecutedAsync(
        SlashCommandInfo commandInfo,
        IInteractionContext context,
        IResult result)
    {
        if (result.IsSuccess)
            return Task.CompletedTask;

        return result switch
        {
            LocalizedPreconditionResult r => context.Interaction.RespondAsync(
                _localizationService.Localize(r.LocalizationKey, r.LocalizationArguments),
                ephemeral: true),
            _ => Task.CompletedTask
        };
    }
}