using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

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