using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiSphere;

public class FlexiSphereException : ApplicationException
{
    public FlexiSphereException()
    {
    }

    public FlexiSphereException(string? message)
        : base(message)
    {
    }

    public FlexiSphereException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
