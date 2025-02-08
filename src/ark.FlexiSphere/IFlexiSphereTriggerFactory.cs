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
// FileName: IFlexiSphereTriggerFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.092Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace ark.FlexiSphere;

public interface IFlexiSphereTriggerFactory : IFlexiSphereFactory
{
    /// <summary>
    /// Set the owener for the trigger
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory SetOwner(IFlexiSphereComponentFactory owner);

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
    IFlexiSphereTriggerFactory SetMaxConcurrents(int maxConcurrent);

    /// <summary>
    /// Define the action to be executed <c ref="FlexiSphereEventTrigger" />
    /// </summary>
    /// <param name="eventAction"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IFlexiSphereTriggerFactory ActivateOnAction(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000);

    /// <summary>
    /// Define the cron expression for the trigger <c ref="FlexiSphereScheduledTrigger" />
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
