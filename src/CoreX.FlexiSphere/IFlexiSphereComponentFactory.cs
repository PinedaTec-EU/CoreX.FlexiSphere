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
// FileName: IFlexiSphereComponentFactory.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-06T16:38:41.753Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace CoreX.FlexiSphere;

public interface IFlexiSphereComponentFactory : IFlexiSphereFactory
{
    /// <summary>
    /// Add a job to the FlexiSphere
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    IFlexiSphereComponentFactory AddJob(IFlexiSphereJob job);

    /// <summary>
    /// Add a job to the FlexiSphere
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IFlexiSphereComponentFactory AddJob(Action<IFlexiSphereJobFactory> action);

    /// <summary>
    /// Add a trigger to the FlexiSphere
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    IFlexiSphereComponentFactory AddTrigger(IFlexiSphereTrigger trigger);

    /// <summary>
    /// Add a trigger to the FlexiSphere
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IFlexiSphereComponentFactory AddTrigger(Action<IFlexiSphereTriggerFactory> action);

    /// <summary>
    /// Build the FlexiSphere
    /// </summary>
    /// <returns></returns>
    IFlexiSphere Build();
}
