using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public interface IFlexiSphereFactory
{
    /// <summary>
    /// Add a job to the TimeSphere
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    IFlexiSphereFactory AddJob(IFlexiSphereJob job);

    /// <summary>
    /// Add a job to the TimeSphere
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IFlexiSphereFactory AddJob(Action<IFlexiSphereJobFactory> action);

    /// <summary>
    /// Add a trigger to the TimeSphere
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    IFlexiSphereFactory AddTrigger(IFlexiSphereTrigger trigger);

    /// <summary>
    /// Add a trigger to the TimeSphere
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IFlexiSphereFactory AddTrigger(Action<IFlexiSphereTriggerFactory> action);

    /// <summary>
    /// Build the TimeSphere
    /// </summary>
    /// <returns></returns>
    IFlexiSphere Build();
}
