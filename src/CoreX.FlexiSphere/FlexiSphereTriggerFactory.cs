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
// FileName: FlexiSphereTriggerFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.084Z
//
// --------------------------------------------------------------------------------------

#endregion

using CoreX.FlexiSphere.triggers;
using Microsoft.Extensions.Options;

namespace CoreX.FlexiSphere;

public class FlexiSphereTriggerFactory : IFlexiSphereTriggerFactory
{
    private int _maxOccurences = 0;
    private int _maxConcurrents = 1;
    private int _delay = 1000;
    private Func<IFlexiSphereContext?, Task<bool>>? _eventAction;
    private string? _cronExpression = null;

    private string? _triggerName;
    private string? _triggerGroup;

    private bool _fireOnStart = false;

    public static IFlexiSphereTriggerFactory Create() =>
        new FlexiSphereTriggerFactory();

    public FlexiSphereTriggerFactory()
    { }

    public FlexiSphereTriggerFactory(IOptions<FlexiSphereTriggerFactoryOptions>? options)
        : this()
    {
        if (options is not null)
        {
            _maxOccurences = options.Value.MaxOccurrences;
            _maxConcurrents = options.Value.MaxConcurrents;
            _fireOnStart = options.Value.FireTriggerOnStart;
        }
    }

    public FlexiSphereTriggerFactory(FlexiSphereTriggerFactoryOptions options)
        : this()
    {
        if (options is not null)
        {
            _maxOccurences = options.MaxOccurrences;
            _maxConcurrents = options.MaxConcurrents;
            _fireOnStart = options.FireTriggerOnStart;
        }
    }

    public IFlexiSphereTriggerFactory SetOwner(IFlexiSphereComponentFactory owner)
    {
        return this;
    }

    public IFlexiSphereTriggerFactory WithTriggerName(string triggerName, string triggerGroup)
    {
        triggerName.ThrowExceptionIfNullOrEmpty<FlexiSphereException>($"{nameof(triggerName)} cannot be null or empty!");

        _triggerName = triggerName;
        _triggerGroup = triggerGroup;

        return this;
    }

    public IFlexiSphereTriggerFactory SetMaxOccurences(int maxOccurences)
    {
        _maxOccurences = maxOccurences;
        return this;
    }

    public IFlexiSphereTriggerFactory SetMaxConcurrents(int maxConcurrent)
    {
        _maxConcurrents = maxConcurrent;
        return this;
    }

    public IFlexiSphereTriggerFactory ActivateOnAction(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000)
    {
        _eventAction = eventAction;

        _cronExpression.ThrowExceptionIfNotNullOrEmpty<FlexiSphereException>("CronExpression is defined!");
        _eventAction.ThrowExceptionIfNull<FlexiSphereException>($"{nameof(eventAction)} cannot be null!");

        _delay = delay;
        return this;
    }

    public IFlexiSphereTriggerFactory StartOn(string cronExpression)
    {
        _cronExpression = cronExpression;

        _eventAction.ThrowExceptionIfNotNull<FlexiSphereException>("EventAction is defined!");
        _cronExpression.ThrowExceptionIfNullOrEmpty<FlexiSphereException>($"{nameof(cronExpression)} cannot be null or empty!");

        return this;
    }

    public IFlexiSphereTrigger Build()
    {
        if (_cronExpression != null)
        {
            _cronExpression.ThrowExceptionIfNullOrEmpty<FlexiSphereException>($"{nameof(_cronExpression)} cannot be null or empty!");
            return this.CreateScheduledTrigger();
        }

        _eventAction.ThrowExceptionIfNull<FlexiSphereException>($"{nameof(_eventAction)} cannot be null!");
        return this.CreateEventTrigger();
    }

    private IFlexiSphereTrigger CreateEventTrigger()
    {
        var trigger = new FlexiSphereEventTrigger();
        trigger.ConfigureTrigger(_eventAction!, _delay, _maxConcurrents, _maxOccurences);
        trigger.TriggerName = _triggerName!;

        if (_fireOnStart)
        {
            trigger.FireTriggerOnStart = true;
        }

        return trigger;
    }

    private IFlexiSphereTrigger CreateScheduledTrigger()
    {
        var trigger = new FlexiSphereScheduledTrigger();
        trigger.ConfigureTrigger(_cronExpression, _maxConcurrents, _maxOccurences);
        trigger.TriggerName = _triggerName!;

        if (_fireOnStart)
        {
            trigger.FireTriggerOnStart = true;
        }

        return trigger;
    }

    public IFlexiSphereTriggerFactory FireTriggerOnStart(bool activate)
    {
        _fireOnStart = activate;

        return this;
    }
}
