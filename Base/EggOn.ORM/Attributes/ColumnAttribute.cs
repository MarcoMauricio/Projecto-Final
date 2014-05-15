using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    /// <summary>
    /// For explicit poco properties, marks the property as a column and optionally 
    /// supplies the DB column name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
            ForceToUtc = false;
        }

        public ColumnAttribute(string Name)
        {
            this.Name = Name;
            ForceToUtc = false;
        }

        public string Name
        {
            get;
            set;
        }

        public bool ForceToUtc
        {
            get;
            set;
        }
    }
}
