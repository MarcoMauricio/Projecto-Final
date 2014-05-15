using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value
        {
            get;
            set;
        }

        public DefaultValueAttribute(object Value) 
		{
            this.Value = Value;
		}
    }
}
