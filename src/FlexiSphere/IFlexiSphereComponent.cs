using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public interface IFlexiSphereComponent
{

}

public delegate void FlexiSphereHandler(IFlexiSphereComponent sender, IFlexiSphereContext? context);
public delegate void FlexiSphereExceptionHandler<T>(IFlexiSphereComponent sender, IFlexiSphereContext? context, T exception) where T : Exception;
