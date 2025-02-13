#region Header

// --------------------------------------------------------------------------------------
// Powered by:
// 
//     __________.__                  .___    ___________                             
//     \______   \__| ____   ____   __| _/____\__    ___/___   ____       ____  __ __ 
//      |     ___/  |/    \_/ __ \ / __ |\__  \ |    |_/ __ \_/ ___\    _/ __ \|  |  \
//      |    |   |  |   |  \  ___// /_/ | / __ \|    |\  ___/\  \___    \  ___/|  |  /
//      |____|   |__|___|  /\___  >____ |(____  /____| \___  >\___  > /\ \___  >____/ 
//                   \/     \/     \/     \/           \/     \/  \/     \/
// 
// 
// FileName: FlexiSphereExtensions.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-06T16:38:41.751Z
//
// --------------------------------------------------------------------------------------

#endregion

using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using CoreX.FlexiSphere.jobs;
using CoreX.FlexiSphere.triggers;

namespace CoreX.FlexiSphere;

public static class FlexiSphereExtensions
{
    public static IServiceCollection AddFlexiSphere(this IServiceCollection services, Action<FlexiSphereFactoryOptions>? configureOptions = null) =>
        services.AddFlexiSphere<FlexiSphereFactory, FlexiSphere, FlexiSphereTriggerFactory, FlexiSphereJobFactory>(configureOptions);

    public static IServiceCollection AddFlexiSphere<FSF>(this IServiceCollection services, Action<FlexiSphereFactoryOptions>? configureOptions = null)
        where FSF : class, IFlexiSphereComponent
    {
        ConfigureOptions(services, configureOptions);
        services.AddFlexiSphere<FlexiSphereFactory, FlexiSphere, FlexiSphereTriggerFactory, FlexiSphereJobFactory>(configureOptions);

        Type fsfType = typeof(FSF);

        if (fsfType.GetInterfaces().Any(fi => fi == typeof(IFlexiSphereComponentFactory)))
        {
            services.AddTransient(typeof(IFlexiSphereComponentFactory), typeof(FSF));
        }
        else if (fsfType.GetInterfaces().Any(fi => fi == typeof(IFlexiSphereTriggerFactory)))
        {
            services.AddTransient(typeof(IFlexiSphereTriggerFactory), typeof(FSF));
        }
        else if (fsfType.GetInterfaces().Any(fi => fi == typeof(IFlexiSphereJobFactory)))
        {
            services.AddTransient(typeof(IFlexiSphereJobFactory), typeof(FSF));
        }
        else if (fsfType.GetInterfaces().Any(fi => fi == typeof(IFlexiSphere)))
        {
            services.AddTransient(typeof(IFlexiSphere), typeof(FSF));
        }
        else
        {
            throw new NotSupportedException("Unsupported FlexiSphereFactory type!");
        }

        return services;
    }

    public static IServiceCollection AddFlexiSphere<FSF, FS, FSTF, FSJF>(this IServiceCollection services, Action<FlexiSphereFactoryOptions>? configureOptions = null)
        where FSF : class, IFlexiSphereComponentFactory
        where FS : class, IFlexiSphere
        where FSTF : class, IFlexiSphereTriggerFactory
        where FSJF : class, IFlexiSphereJobFactory
    {
        ConfigureOptions(services, configureOptions);

        services.AddTransient<IFlexiSphereComponentFactory, FSF>();
        services.AddTransient<IFlexiSphereTriggerFactory, FSTF>();
        services.AddTransient<IFlexiSphereJobFactory, FSJF>();

        services.AddTransient<IFlexiSphere, FS>();

        return services;
    }

    private static void ConfigureOptions(IServiceCollection services, Action<FlexiSphereFactoryOptions>? configureOptions)
    {
        services.AddOptions();

        if (configureOptions is not null)
        {
            services.Configure<FlexiSphereFactoryOptions>(configureOptions);

            var serviceProvider = services.BuildServiceProvider();
            var mainOptions = serviceProvider.GetService<IOptions<FlexiSphereFactoryOptions>>()?.Value;

            if (mainOptions != null)
            {
                if (mainOptions.TriggerFactoryOptions is not null)
                {
                    services.Configure<FlexiSphereTriggerFactoryOptions>(opt =>
                    {
                        opt.FireTriggerOnStart = mainOptions.TriggerFactoryOptions.FireTriggerOnStart;
                        opt.MaxOccurrences = mainOptions.TriggerFactoryOptions.MaxOccurrences;
                        opt.MaxConcurrents = mainOptions.TriggerFactoryOptions.MaxConcurrents;
                    });
                }

                if (mainOptions.JobFactoryOptions is not null)
                {
                    services.Configure<FlexiSphereJobFactoryOptions>(opt =>
                    {
                        opt.RateLimiter = mainOptions.JobFactoryOptions.RateLimiter;
                        opt.MaxConcurrents = mainOptions.JobFactoryOptions.MaxConcurrents;
                    });
                }
            }
        }
    }
}

public class FlexiSphereFactoryOptions
{
    public FlexiSphereTriggerFactoryOptions? TriggerFactoryOptions { get; set; }
    public FlexiSphereJobFactoryOptions? JobFactoryOptions { get; set; }
}

public class FlexiSphereTriggerFactoryOptions
{
    public bool FireTriggerOnStart { get; set; } = false;
    public int MaxOccurrences { get; set; } = 0;
    public int MaxConcurrents { get; set; } = 1;
}

public class FlexiSphereJobFactoryOptions
{
    public TimeSpan? RateLimiter { get; set; }
    public int MaxConcurrents { get; set; } = 1;
}