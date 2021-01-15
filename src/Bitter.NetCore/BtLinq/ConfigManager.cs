using System;
using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    public class ConfigManager
    {
        private static DatabaseType? databaseType;

        public static string ConnectionStringName { get; internal set; }

        public static string DataBase { get; set; }

        public static DatabaseType DataBaseType
        {
            get
            {
                if (!databaseType.HasValue)
                {
                    var nullable = databaseType = (DatabaseType) Enum.Parse(typeof (DatabaseType), DataBase);
                    return nullable.Value;
                }
                return databaseType.Value;
            }
        }

        public static string DbFactoryName { get; set; }

        public static bool IsEnableAllwayAutoCreateTables { get; set; }

        public static bool IsEnableAutoCreateTables { get; set; }

        public static string SequenceTable { get; internal set; }

        public static string SqlBuilder { get; set; }
    }
}