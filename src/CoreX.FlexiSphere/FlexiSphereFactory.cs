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

using CoreX.extensions;
using Microsoft.Extensions.Options;

namespace CoreX.FlexiSphere;

public class FlexiSphereFactory : IFlexiSphereComponentFactory
{
    private IFlexiSphere _flexiSphere;

    private IFlexiSphereTriggerFactory? _triggerFactory;
    private IFlexiSphereJobFactory? _jobFactory;

    public static IFlexiSphereComponentFactory Create(IFlexiSphere? flexiSphere = null, IFlexiSphereTriggerFactory? triggerFactory = null, IFlexiSphereJobFactory? jobFactory = null) =>
        new FlexiSphereFactory(flexiSphere, triggerFactory, jobFactory);

    public FlexiSphereFactory(IFlexiSphere? flexiSphere = null, IFlexiSphereTriggerFactory? triggerFactory = null, IFlexiSphereJobFactory? jobFactory = null, IOptions<FlexiSphereFactoryOptions>? options = null)
    {
        if (options is not null)
        {
            // Nothing to do here
        }

        triggerFactory ??= new FlexiSphereTriggerFactory();
        jobFactory ??= new FlexiSphereJobFactory();

        _triggerFactory = triggerFactory;
        _triggerFactory.SetOwner(this);

        _jobFactory = jobFactory;
        _jobFactory.SetOwner(this);

        flexiSphere ??= new FlexiSphere();
        _flexiSphere = flexiSphere;
    }

    public IFlexiSphereComponentFactory AddJob(IFlexiSphereJob job)
    {
        job.ThrowArgumentExceptionIfNull(nameof(job));

        _flexiSphere.AddJob(job);
        return this;
    }

    public IFlexiSphereComponentFactory AddJob(Action<IFlexiSphereJobFactory> action)
    {
        action.ThrowArgumentExceptionIfNull(nameof(action));

        var Factory = _jobFactory ?? FlexiSphereJobFactory.Create();
        action(Factory);

        _flexiSphere.AddJob(Factory.Build());

        return this;
    }

    public IFlexiSphereComponentFactory AddTrigger(IFlexiSphereTrigger trigger)
    {
        trigger.ThrowArgumentExceptionIfNull(nameof(trigger));

        _flexiSphere.AddTrigger(trigger);
        return this;
    }

    public IFlexiSphereComponentFactory AddTrigger(Action<IFlexiSphereTriggerFactory> action)
    {
        action.ThrowArgumentExceptionIfNull(nameof(action));

        var Factory = _triggerFactory ?? FlexiSphereTriggerFactory.Create();
        action(Factory);

        _flexiSphere.AddTrigger(Factory.Build());

        return this;
    }

    public IFlexiSphere Build() =>
        _flexiSphere;
}
