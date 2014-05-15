using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.DataHost
{
    /// <summary>
    /// Specifies the primary key column of a poco class, whether the column is auto incrementing
    /// and the sequence name for Oracle sequence columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(string primaryKey)
        {
            Value = primaryKey;
            autoIncrement = true;
        }

        public string Value
        {
            get;
            private set;
        }

        public string sequenceName
        {
            get;
            set;
        }

        public bool autoIncrement
        {
            get;
            set;
        }
    }

}
