using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public class FlexiSphereFactory : IFlexiSphereFactory
{
    private IFlexiSphere _FlexiSphere;

    private IFlexiSphereTriggerFactory? _triggerFactory;
    private IFlexiSphereJobFactory? _jobFactory;

    public static IFlexiSphereFactory Create(IFlexiSphereTriggerFactory? triggerFactory = null, IFlexiSphereJobFactory? jobFactory = null)
    {
        triggerFactory ??= new FlexiSphereTriggerFactory();
        jobFactory ??= new FlexiSphereJobFactory();

        return new FlexiSphereFactory(triggerFactory, jobFactory);
    }

    public FlexiSphereFactory(IFlexiSphereTriggerFactory triggerFactory, IFlexiSphereJobFactory jobFactory)
    {
        _triggerFactory = triggerFactory;
        _triggerFactory.SetOwner(this);

        _jobFactory = jobFactory;
        _jobFactory.SetOwner(this);

        _FlexiSphere = new FlexiSphere();
    }

    public IFlexiSphereFactory AddJob(IFlexiSphereJob job)
    {
        job.ThrowArgumentExceptionIfNull(nameof(job));

        _FlexiSphere.AddJob(job);
        return this;
    }

    public IFlexiSphereFactory AddJob(Action<IFlexiSphereJobFactory> action)
    {
        action.ThrowArgumentExceptionIfNull(nameof(action));

        var Factory = _jobFactory ?? FlexiSphereJobFactory.Create();
        action(Factory);

        _FlexiSphere.AddJob(Factory.Build());

        return this;
    }

    public IFlexiSphereFactory AddTrigger(IFlexiSphereTrigger trigger)
    {
        trigger.ThrowArgumentExceptionIfNull(nameof(trigger));

        _FlexiSphere.AddTrigger(trigger);
        return this;
    }

    public IFlexiSphereFactory AddTrigger(Action<IFlexiSphereTriggerFactory> action)
    {
        action.ThrowArgumentExceptionIfNull(nameof(action));

        var Factory = _triggerFactory ?? FlexiSphereTriggerFactory.Create();
        action(Factory);

        _FlexiSphere.AddTrigger(Factory.Build());

        return this;
    }

    public IFlexiSphere Build() =>
        _FlexiSphere;
}
