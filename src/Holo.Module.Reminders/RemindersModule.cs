using Autofac;
using Holo.Module.Reminders.Configuration;
using Holo.Sdk.Configurations;
using Holo.Sdk.DI;
using Holo.Sdk.Modules;

namespace Holo.Module.Reminders;

public sealed class RemindersModule : ModuleBase
{
    protected override void OnConfigureContainer(
        ContainerBuilder containerBuilder,
        IConfigurationProvider configurationProvider)
    {
        containerBuilder.RegisterOptions<ReminderOptions>(configurationProvider, ReminderOptions.SectionName);
    }
}