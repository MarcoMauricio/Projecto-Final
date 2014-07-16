using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace FlowOptions.EggOn.DataHost
{
    public class EggOnDatabase : Database
    {
        public static bool ALLOW_DATA_LOSS_ON_UPDATE { get; set; }

        public EggOnDatabase() : base("EggOnConnection")
        {
            EnableAutoSelect = true;
            EnableNamedParams = false;

#if DEBUG
            ALLOW_DATA_LOSS_ON_UPDATE = true;
#else
            EggOnDatabase.ALLOW_DATA_LOSS_ON_UPDATE = false;
#endif
        }

        public bool CreateOrUpdateTableFromModel(Type t)
        {
            var tnAttribute = t.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            if (tnAttribute == null)
            {
                throw new Exception("To automatically create a table,  Model requires a TableName attribute.");
            }

            var tableName = tnAttribute.Value;
            
            var pkAttribute = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() as PrimaryKeyAttribute;

            var columns = new List<SqlColumn>();

            foreach (var prop in t.GetProperties())
            {
                var ignoreAttribute = prop.GetCustomAttributes(typeof(IgnoreAttribute), true).FirstOrDefault() as IgnoreAttribute;
                if (ignoreAttribute != null)
                {
                    continue;
                }

                var columnName = prop.Name;

                var columnAttribute = prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                if (columnAttribute != null && columnAttribute.Name != null)
                {
                    columnName = columnAttribute.Name;
                }

                var column = new SqlColumn();
                column.Name = columnName;
                column.Type = GetSqlTypeFromType(prop.PropertyType);

                if (column.Type == null)
                {
                    throw new Exception(string.Format("Property type from column {0} is not supported.", prop.Name));
                }

                if (pkAttribute != null && pkAttribute.Value == prop.Name)
                {
                    column.PrimaryKey = true;
                }

                column.AllowsNulls = (prop.GetCustomAttributes(typeof(AllowNullAttribute), true).FirstOrDefault() != null);

                column.Unique = (prop.GetCustomAttributes(typeof(UniqueAttribute), true).FirstOrDefault() != null);

                var defaultValueAttribute = prop.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault() as DefaultValueAttribute;
                if (defaultValueAttribute != null)
                {
                    column.DefaultValue = defaultValueAttribute.Value;
                }

                var constraintAttribute = prop.GetCustomAttributes(typeof(ConstraintAttribute), true).FirstOrDefault() as ConstraintAttribute;
                if (constraintAttribute != null)
                {
                    var ftnAttribute = constraintAttribute.ForeignObject.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
                    if (ftnAttribute == null)
                    {
                        throw new Exception("Can't create constraint to a model without a table.");
                    }

                    column.Constraint = new SqlConstraint(ftnAttribute.Value, constraintAttribute.ForeignProperty);
                }

                columns.Add(column);
            }

            if (columns.Count == 0)
            {
                throw new Exception("Model doesn't have public properties.");
            }

            return CreateOrUpdateTable(tableName, columns);
        }
       
        // TODO: Update table.
        public bool CreateOrUpdateTable(string tableName, IEnumerable<SqlColumn> columns)
        {
            using (var tr = GetTransaction())
            {
                var schemaName = GetSchemaFromTableName(tableName);

                // TODO: Update table schema.
                if (TableExists(tableName))
                {
                    return false;
                }

                if (!SchemaExists(schemaName))
                {
                    Execute(@"Create SCHEMA " + schemaName);
                }

                var contraints = new LinkedList<SqlColumn>();
                var uniques = new LinkedList<SqlColumn>();
                
                // TODO: Multi column keys.
                SqlColumn primaryKey = null;

                var sb = new StringBuilder();

                sb.AppendLine("CREATE TABLE " + CleanTableName(tableName) + " (");

                var index = 0;

                foreach(var column in columns)
                {
                    if (index++ > 0)
                        sb.AppendLine(", ");

                    sb.Append(CleanColumnName(column.Name) + " " + CleanColumnType(column.Type) + " ");
                    
                    if (column.DefaultValue != null)
                    {
                        sb.Append("DEFAULT '" + column.DefaultValue + "' ");
                    }
                    sb.Append((column.AllowsNulls) ? "NULL" : "NOT NULL");

                    if (column.PrimaryKey && primaryKey == null)
                    {
                        primaryKey = column;
                    }

                    if (column.Constraint != null)
                    {
                        contraints.AddLast(column);
                    }

                    if (column.Unique)
                    {
                        uniques.AddLast(column);
                    }
                }

                if (primaryKey != null)
                {
                    sb.AppendLine(", ");
                    sb.AppendLine("CONSTRAINT " + GenerateKeyName("PK", tableName, new List<SqlColumn>() { primaryKey }) + " PRIMARY KEY CLUSTERED");
                    sb.AppendLine("(" + CleanColumnName(primaryKey.Name) + " ASC)");
                }

                /*
                if (uniques.Count != 0)
                {
                    sb.AppendLine(", ");
                    sb.AppendLine("CONSTRAINT " + GenerateKeyName("UQ", tableName, uniques) + " UNIQUE NONCLUSTERED");

                    sb.Append("(");
                    index = 0;
                    foreach (SqlColumn column in uniques)
                    {
                        if (index++ > 0)
                            sb.Append(", ");

                        sb.Append(CleanColumnName(column.Name));
                    }
                    sb.AppendLine(")");
                }
                */
                sb.AppendLine(");");

                Execute(sb.ToString());

                foreach (var column in contraints)
                {
                    sb.Clear();

                    sb.AppendLine("ALTER TABLE " + CleanTableName(tableName) + " WITH CHECK");
                    sb.AppendLine("ADD CONSTRAINT " + GenerateContraintName(tableName, column));
                    sb.AppendLine("FOREIGN KEY (" + CleanColumnName(column.Name) + ")");
                    sb.AppendLine("REFERENCES " + CleanTableName(column.Constraint.TableName) + " (" + CleanColumnName(column.Constraint.ColumnName) + ")");

                    Execute(sb.ToString());
                }

                tr.Complete();
            }

            return true;
        }


        public void DropTable(string tableName)
        {
            Execute("DROP TABLE " + tableName + ";");
        }

        public bool TableExists(string tableName)
        {
            var schemaName = "dbo";

            if (tableName.IndexOf('.') != -1) {
                var result = tableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                schemaName = result[0];
                tableName = result[1];
            }

            schemaName = schemaName.Trim(' ', '[', ']');
            tableName = tableName.Trim(' ', '[', ']');

            return ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @0 AND TABLE_NAME = @1", schemaName, tableName) != 0;
        }

        public bool SchemaExists(string schemaName)
        {
            schemaName = schemaName.Trim(' ', '[', ']');

            return ExecuteScalar<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @0", schemaName) != 0;
        }

        public string GetSchemaFromTableName(string tableName)
        {
            var schemaName = "dbo";

            if (tableName.IndexOf('.') != -1)
            {
                var result = tableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                schemaName = result[0];
                tableName = result[1];
            }

            schemaName = schemaName.Trim(' ', '[', ']');

            return schemaName;
        }


        public string CleanTableName(string tableName)
        {
            if (tableName.IndexOf('.') != -1)
            {
                var result = tableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                return string.Format("[{0}].[{1}]", result[0].Trim(' ', '[', ']'), result[1].Trim(' ', '[', ']'));
            }

            return string.Format("[dbo].[{0}]", tableName.Trim(' ', '[', ']'));
        }

        public string CleanColumnName(string columnName)
        {
            return string.Format("[{0}]", columnName.Trim(' ', '[', ']'));
        }

        public string CleanColumnType(string columnType)
        {
            if (columnType.IndexOf('(') != -1)
            {
                var result = columnType.Split(new char[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);
                return string.Format("[{0}]({1})", result[0].Trim(' ', '[', ']'), result[1].Trim(' ', '[', ']', ')').ToUpperInvariant());
            }

            return string.Format("[{0}]", columnType.Trim(' ', '[', ']'));
        }


        internal string GenerateKeyName(string prefix, string tableName, IEnumerable<SqlColumn> columns)
        {
            if (tableName.IndexOf('.') != -1)
            {
                var result = tableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                tableName = string.Format("{0}.{1}", result[0].Trim(' ', '[', ']'), result[1].Trim(' ', '[', ']'));
            }
            else
            {
                tableName = string.Format("dbo.{0}", tableName.Trim(' ', '[', ']'));
            }

            var columnNames = columns.Select(col => col.Name);

            tableName = tableName.Trim(' ', '[', ']');

            return string.Format("[{0}_{1}_{2}]", prefix, tableName, String.Join("_", columnNames));
        }

        internal string GenerateContraintName(string tableName, SqlColumn column)
        {
            var foreignTableName = column.Constraint.TableName.Trim(' ', '[', ']').Replace(' ', '_');
            if (foreignTableName.IndexOf('.') != -1)
            {
                var result = foreignTableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                foreignTableName = string.Format("{0}.{1}", result[0].Trim(' ', '[', ']'), result[1].Trim(' ', '[', ']'));
            }

            tableName = tableName.Trim(' ', '[', ']').Replace(' ', '_');
            if (tableName.IndexOf('.') != -1)
            {
                var result = tableName.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                tableName = string.Format("{0}.{1}", result[0].Trim(' ', '[', ']'), result[1].Trim(' ', '[', ']'));
            }

            var columnName = column.Name.Trim(' ', '[', ']').Replace(' ', '_');

            return string.Format("[FK_{0}_{1}_{2}]", foreignTableName, tableName, columnName);
        }


        internal string GetSqlTypeFromType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            switch (type.FullName)
            {
                case "System.Boolean":
                    return "bit";
                case "System.Byte":
                    return "tinyint";
                case "System.Int16":
                case "System.SByte":
                    return "smallint";
                case "System.Int32":
                case "System.UInt16":
                    return "int";
                case "System.Int64":
                case "System.UInt32":
                    return "bigint";
                case "System.UInt64":
                    return "decimal(20)";
                case "System.Decimal":
                    return "decimal(29)";
                case "System.Single":
                    return "real";
                case "System.Double":
                    return "float";
                case "System.Char":
                    return "char(1)";
                case "System.String":
                case "System.Char[]":
                case "System.Xml.Linq.XDocument":
                case "System.Xml.Linq.XElement":
                    return "nvarchar(MAX)";
                case "System.DateTime":
                    return "datetime2";
                case "System.DateTimeOffset":
                    return "datetimeoffset";
                case "System.TimeSpan":
                    return "time";
                case "System.Binary":
                case "System.Byte[]":
                    return "varbinary(max)";
                case "System.Guid":
                    return "uniqueidentifier";
            }

            if (type.IsEnum)
            {
                return "int";
            }

            if (type.IsSerializable)
            {
                return "varbinary(max)";
            }

            return null;
        }
    }

    public class SqlColumn
    {
        public string Name;
        public string Type;
        public bool AllowsNulls;
        public object DefaultValue;
        public bool PrimaryKey;
        public bool Unique;
        public SqlConstraint Constraint;
    }

    public class SqlConstraint
    {
        public string TableName;
        public string ColumnName;

        public SqlConstraint(string tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}