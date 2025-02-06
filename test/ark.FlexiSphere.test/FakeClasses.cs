

using System.Diagnostics.CodeAnalysis;

namespace ark.FlexiSphere.test;

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphereJobFactory : IFlexiSphereJobFactory
{
    public IFlexiSphereJob Build()
    {
        return new FakeClass_FlexiSphereJob();
    }

    public IFlexiSphereJobFactory DefineJob(Type jobType)
    {
        return this;
    }

    public IFlexiSphereJobFactory SetJobAction(Func<IFlexiSphereContext?, Task> jobAction)
    {
        return this;
    }

    public IFlexiSphereJobFactory SetMaxConcurrents(int maxConcurrent)
    {
        return this;
    }

    public IFlexiSphereJobFactory SetOwner(IFlexiSphereComponentFactory owner)
    {
        return this;
    }

    public IFlexiSphereJobFactory SetRateLimiter(TimeSpan rateLimiter, int maxConcurrents)
    {
        return this;
    }

    public IFlexiSphereJobFactory WithJobName(string jobName, string? jobGroup)
    {
        return this;
    }

    IFlexiSphereJobFactory IFlexiSphereJobFactory.DefineJob<TType>(TType jobInstance)
    {
        return this;
    }
}

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphereJob : IFlexiSphereJob
{
    public Ulid Id => throw new NotImplementedException();

    public int MaxConcurrents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string JobName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string? JobGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsRateLimiterEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event FlexiSphereJobExceptionHandler<Exception>? OnFaulted;

    public void ConfigureJob(string jobName, string? jobGroup, int maxConcurrents = 1, TimeSpan? rateLimiter = null)
    {
        throw new NotImplementedException();
    }

    public void ConfigureJob(string jobName, string? jobGroup, Func<IFlexiSphereContext?, Task> jobAction, int maxConcurrents = 1, TimeSpan? rateLimiter = null)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphereTriggerFactory : IFlexiSphereTriggerFactory
{
    public IFlexiSphereTriggerFactory ActivateOnAction(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000)
    {
        return this;
    }

    public IFlexiSphereTrigger Build()
    {
        return new FakeClass_FlexiSphereTrigger();
    }

    public IFlexiSphereTriggerFactory FireTriggerOnStart(bool activate)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory SetMaxConcurrent(int maxConcurrent)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory SetMaxOccurences(int maxOccurences)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory SetOwner(IFlexiSphereComponentFactory owner)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory StartOn(string cronExpression)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory WithTriggerName(string triggerName, string triggerGroup)
    {
        return this;
    }
}

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphereTrigger : IFlexiSphereTrigger
{
    public bool FireTriggerOnStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string TriggerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int MaxOccurrences { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int MaxConcurrents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Counter => throw new NotImplementedException();

    public int PressureCounter => throw new NotImplementedException();

    public event FlexiSphereTriggerEventHandler? OnTriggered;
    public event FlexiSphereTriggerEventHandler? OnCanceled;
    public event FlexiSphereTriggerEventHandler? OnCompleted;
    public event FlexiSphereTriggerExceptionHandler<FlexiSphereException>? OnFaulted;

    public void ActivateTrigger(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void DeactivateTrigger(string issue, IFlexiSphereContext? context = null)
    {
        throw new NotImplementedException();
    }
}

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphereFactory : IFlexiSphereComponentFactory
{
    public IFlexiSphereComponentFactory AddJob(IFlexiSphereJob job)
    {
        return this;
    }

    public IFlexiSphereComponentFactory AddJob(Action<IFlexiSphereJobFactory> action)
    {
        return this;
    }

    public IFlexiSphereComponentFactory AddTrigger(IFlexiSphereTrigger trigger)
    {
        return this;
    }

    public IFlexiSphereComponentFactory AddTrigger(Action<IFlexiSphereTriggerFactory> action)
    {
        return this;
    }

    public IFlexiSphere Build()
    {
        return new FakeClass_FlexiSphere();
    }
}

[ExcludeFromCodeCoverage]
public class FakeClass_FlexiSphere : IFlexiSphere
{
    public int Counter => throw new NotImplementedException();

    public DateTime? LastTriggered => throw new NotImplementedException();

    public IReadOnlyCollection<IFlexiSphereTrigger> Triggers => throw new NotImplementedException();

    public IReadOnlyCollection<IFlexiSphereJob> Jobs => throw new NotImplementedException();

    public event FlexiSphereHandler? OnCanceled;
    public event FlexiSphereHandler? OnCompleted;
    public event FlexiSphereHandler? OnTriggered;
    public event FlexiSphereExceptionHandler<Exception>? OnFaulted;
    public event FlexiSphereHandler? OnBeforeJobExecuted;
    public event FlexiSphereHandler? OnAfterJobExecuted;

    public void AddJob(IFlexiSphereJob job)
    {
        throw new NotImplementedException();
    }

    public void AddTrigger(IFlexiSphereTrigger trigger)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(string issue, IFlexiSphereContext? context = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}