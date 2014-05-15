using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    /// <summary>
    /// Use the Ignore attribute on POCO class properties that shouldn't be mapped
    /// by PetaPoco.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
