using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public interface IFlexiSphereTriggerFactory
{
    /// <summary>
    /// Set the owener for the trigger
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory SetOwner(IFlexiSphereFactory owner);

    /// <summary>
    /// Set the trigger name and group
    /// </summary>
    /// <param name="triggerName"></param>
    /// <param name="triggerGroup"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory WithTriggerName(string triggerName, string triggerGroup);

    /// <summary>
    /// Set the max occurences for the trigger
    /// </summary>
    /// <param name="maxOccurences"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory SetMaxOccurences(int maxOccurences);

    /// <summary>
    /// Set the max concurrent for the trigger
    /// </summary>
    /// <param name="maxConcurrent"></param>
    /// <returns></returns> 
    IFlexiSphereTriggerFactory SetMaxConcurrent(int maxConcurrent);

    /// <summary>
    /// Define the action to be executed <c ref="TimeSphereEventTrigger" />
    /// </summary>
    /// <param name="eventAction"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory ActivateOnAction(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000);

    /// <summary>
    /// Define the cron expression for the trigger <c ref="TimeSphereScheduledTrigger" />
    /// </summary>
    /// <param name="cronExpression"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory StartOn(string cronExpression);

    /// <summary>
    /// Allow the trigger to fire on start
    /// </summary>
    /// <param name="activate"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory FireTriggerOnStart(bool activate);

    /// <summary>
    /// Build the trigger
    /// </summary>
    /// <returns></returns>
    IFlexiSphereTrigger Build();
}
