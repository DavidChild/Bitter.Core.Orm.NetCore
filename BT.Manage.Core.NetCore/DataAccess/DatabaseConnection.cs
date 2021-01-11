namespace BT.Manage.Core
{
    public struct DatabaseConnection
    {
        private string connectionString;
        private DatabaseType databaseType;

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        public DatabaseType DatabaseType
        {
            get
            {
                return this.databaseType;
            }
            set
            {
                this.databaseType = value;
            }
        }
    }
}