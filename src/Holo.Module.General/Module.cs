using Autofac;
using Holo.Module.General.Cookies.Configuration;
using Holo.Module.General.Dice.Configuration;
using Holo.Module.General.UserInfo.Configuration;
using Holo.Sdk.Configurations;
using Holo.Sdk.DI;
using Holo.Sdk.Modules;

namespace Holo.Module.General;

public sealed class Module : ModuleBase
{
    protected override void OnConfigure(ContainerBuilder containerBuilder, IConfigurationProvider configurationProvider)
    {
        containerBuilder.RegisterOptions<FortuneCookieOptions>(configurationProvider, FortuneCookieOptions.SectionName);
        containerBuilder.RegisterOptions<DiceOptions>(configurationProvider, DiceOptions.SectionName);
        containerBuilder.RegisterOptions<AvatarOptions>(configurationProvider, AvatarOptions.SectionName);
    }
}