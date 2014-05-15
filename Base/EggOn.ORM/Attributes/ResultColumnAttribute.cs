using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    /// <summary>
    /// Marks a poco property as a result only column that is populated in queries
    /// but not used for updates or inserts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {
        public ResultColumnAttribute()
        {
        }

        public ResultColumnAttribute(string name)
            : base(name)
        {
        }
    }
}
