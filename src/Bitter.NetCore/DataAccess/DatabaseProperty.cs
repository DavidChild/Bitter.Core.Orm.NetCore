namespace Bitter.Core
{
    public class DatabaseProperty
    {
        private DatabaseConnection reader;
        private DatabaseConnection writer;

        public DatabaseProperty(DatabaseConnection reader, DatabaseConnection writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public string ConnectionWriterString
        {
            get
            {
                return this.writer.ConnectionString;
            }
        }

        public DatabaseConnection Reader
        {
            get
            {
                return this.reader;
            }
        }

        public DatabaseConnection Writer
        {
            get
            {
                return this.writer;
            }
        }
    }
}