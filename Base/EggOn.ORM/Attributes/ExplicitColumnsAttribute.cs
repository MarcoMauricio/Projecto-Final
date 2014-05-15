using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    /// <summary>
    /// Poco classes marked with the Explicit attribute require all column properties to 
    /// be marked with the Column attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute : Attribute
    {
    }
}
