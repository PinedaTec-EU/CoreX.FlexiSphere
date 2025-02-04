using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexiSphere.triggers;

namespace FlexiSphere;

public class FlexiSphereTriggerFactory : IFlexiSphereTriggerFactory
{
    private int _maxOccurences = 0;
    private int _maxConcurrent = 1;
    private int _delay = 1000;
    private Func<IFlexiSphereContext?, Task<bool>>? _eventAction;
    private string? _cronExpression = null;

    private string? _triggerName;
    private string? _triggerGroup;

    private bool _fireOnStart = false;

    public static IFlexiSphereTriggerFactory Create() =>
        new FlexiSphereTriggerFactory();

    public IFlexiSphereTriggerFactory SetOwner(IFlexiSphereFactory owner)
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

    public IFlexiSphereTriggerFactory SetMaxConcurrent(int maxConcurrent)
    {
        _maxConcurrent = maxConcurrent;
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
        trigger.ConfigureTrigger(_eventAction!, _delay, _maxConcurrent, _maxOccurences);
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
        trigger.ConfigureTrigger(_cronExpression, _maxConcurrent, _maxOccurences);
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
