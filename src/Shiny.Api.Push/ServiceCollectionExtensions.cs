﻿namespace Shiny.Api.Push;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shiny.Api.Push.Extensions;
using Shiny.Api.Push.Infrastructure;
using Shiny.Api.Push.Providers;


public static class ServiceCollectionExtensions
{
    public static void AddPushManagement(this IServiceCollection services, Action<PushConfigurator> configure)
    {
        var cfg = new PushConfigurator(services);
        configure(cfg);

        services.TryAddSingleton<IPushManager, PushManager>();
    }


    public static PushConfigurator AddPerformanceLogger(this PushConfigurator pushConfig)
    {
        pushConfig.Services.AddSingleton<INotificationReporter, BatchTimeNotificationReporter>();
        return pushConfig;
    }


    public static PushConfigurator AddApplePush(this PushConfigurator pushConfig, AppleConfiguration configuration)
    {
        pushConfig.Services.AddSingleton<IApplePushProvider>(x => new ApplePushProvider(configuration));
        return pushConfig;
    }


    public static PushConfigurator AddGooglePush(this PushConfigurator pushConfig, GoogleConfiguration configuration)
    {
        pushConfig.Services.AddSingleton<IGooglePushProvider>(x => new GooglePushProvider(configuration));
        return pushConfig;
    }
}
