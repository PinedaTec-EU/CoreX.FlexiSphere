namespace ark.FlexiSphere;

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
