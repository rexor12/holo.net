using System;
using System.Net.Http;
using Autofac;
using Holo.Module.General.Cookies.Configuration;
using Holo.Module.General.Dice.Configuration;
using Holo.Module.General.Reactions.Configurations;
using Holo.Module.General.UserInfo.Configuration;
using Holo.Sdk.Configurations;
using Holo.Sdk.DI;
using Holo.Sdk.Modules;
using Polly;
using Polly.Extensions.Http;

namespace Holo.Module.General;

public sealed class Module : ModuleBase
{
    protected override void OnConfigureContainer(
        ContainerBuilder containerBuilder,
        IConfigurationProvider configurationProvider)
    {
        containerBuilder.RegisterOptions<FortuneCookieOptions>(configurationProvider, FortuneCookieOptions.SectionName);
        containerBuilder.RegisterOptions<DiceOptions>(configurationProvider, DiceOptions.SectionName);
        containerBuilder.RegisterOptions<AvatarOptions>(configurationProvider, AvatarOptions.SectionName);
        containerBuilder.RegisterOptions<ReactionOptions>(configurationProvider, ReactionOptions.SectionName);
    }

    protected override void OnConfigureHttpClients(
        AddHttpClientDelegate addHttpClient,
        IConfigurationProvider configurationProvider)
    {
        addHttpClient(
            "WaifuPics",
            () =>
            {
                var rateLimiter = Policy.RateLimitAsync<HttpResponseMessage>(
                    configurationProvider.GetValue<int>("Extensions:General:ReactionOptions:RateLimiterRequestsPerInterval"),
                    TimeSpan.FromSeconds(configurationProvider.GetValue<int>("Extensions:General:ReactionOptions:RateLimiterIntervalInSeconds")));
                var circuitBreaker = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        configurationProvider.GetValue<int>("Extensions:General:ReactionOptions:CircuitBreakerFailureThreshold"),
                        TimeSpan.FromSeconds(configurationProvider.GetValue<int>("Extensions:General:ReactionOptions:CircuitBreakerRecoveryTimeInSeconds")));

                return Policy.WrapAsync(rateLimiter, circuitBreaker);
            },
            c =>
            {
                c.BaseAddress = new Uri(configurationProvider.GetValue<string>("Extensions:General:ReactionOptions:ApiBaseUrl"));
            });
    }
}