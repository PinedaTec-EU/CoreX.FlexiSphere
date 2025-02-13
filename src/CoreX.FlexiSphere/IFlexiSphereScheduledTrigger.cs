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
// FileName: IFLexiSphereScheduledTrigger.cs
//
// Author:   jmr.pineda
// eMail:    jmr.pineda@pinedatec.eu
// Profile:  http://pinedatec.eu/profile
//
//           Copyrights (c) PinedaTec.eu 2025, all rights reserved.
//           CC BY-NC-ND - https://creativecommons.org/licenses/by-nc-nd/4.0
//
//  Created at: 2025-02-06T04:42:24.478Z
//
// --------------------------------------------------------------------------------------

#endregion

namespace CoreX.FlexiSphere;

public interface IFlexiSphereScheduledTrigger : IFlexiSphereTrigger
{
    /// <summary>
    /// Gets the cron time expression
    /// </summary>
    string CronTime { get; }

    /// <summary>
    /// Gets the next occurrence of the trigger
    /// </summary>
    /// <returns></returns>
    DateTime? GetNextOccurrence();

    /// <summary>
    /// Configures the trigger
    /// </summary>
    /// <param name="cronExpression"></param>
    /// <param name="maxConcurrent"></param>
    /// <param name="maxOccurences"></param>
    void ConfigureTrigger(string? cronExpression, int maxConcurrent = 100, int maxOccurences = 0);
}
