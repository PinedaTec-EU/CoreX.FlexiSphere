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
// FileName: IFlexiSphereEventTrigger.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-04T13:18:01.088Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace ark.FlexiSphere;

public interface IFlexiSphereEventTrigger : IFlexiSphereTrigger
{
    /// <summary>
    /// Configure the trigger
    /// </summary>
    /// <param name="eventAction"></param>
    /// <param name="delay"></param>
    /// <param name="maxConcurrent"></param>
    /// <param name="maxOccurences"></param>
    void ConfigureTrigger(Func<IFlexiSphereContext?, Task<bool>> eventAction, int delay = 1000, int maxConcurrent = 1, int maxOccurences = 0);
}