using Microsoft.Extensions.DependencyInjection;

using ark.FlexiSphere.jobs;
using ark.FlexiSphere.triggers;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;

namespace ark.FlexiSphere;

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
    public bool FireTriggerOnStart { get; set; }
    public int MaxOccurrences { get; set; }
    public int MaxConcurrents { get; set; }
}

public class FlexiSphereJobFactoryOptions
{
    public TimeSpan? RateLimiter { get; set; }
    public int MaxConcurrents { get; set; } = 1;
}