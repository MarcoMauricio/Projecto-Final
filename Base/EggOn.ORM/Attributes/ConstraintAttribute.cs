using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConstraintAttribute : Attribute
    {
        public Type ForeignObject
        {
            get;
            set;
        }

        public string ForeignProperty
        {
            get;
            set;
        }

        public ConstraintAttribute(Type ForeignObject) : this(ForeignObject, "Id")
        {

        }

        public ConstraintAttribute(Type ForeignObject, string ForeignProperty) 
		{
            this.ForeignObject = ForeignObject;
            this.ForeignProperty = ForeignProperty;
		}
    }
}
