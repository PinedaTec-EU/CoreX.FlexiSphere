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
// FileName: FlexiSphereFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.082Z
//
// --------------------------------------------------------------------------------------

#endregion

using ark.extensions;

namespace ark.FlexiSphere;

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
