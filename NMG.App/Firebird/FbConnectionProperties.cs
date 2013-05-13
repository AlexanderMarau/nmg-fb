using FirebirdSql.Data.FirebirdClient;

namespace NHibernateMappingGenerator.Firebird
{
    public class FbConnectionProperties
    {
        private FbConnectionStringBuilder connectionStringBuilder;

        public string ConnectionString
        {
            get { return connectionStringBuilder.ToString();  }
        }

        public FbConnectionProperties()
        {
            connectionStringBuilder = new FbConnectionStringBuilder();
        }

        public FbConnectionProperties(string connectionString)
        {
            connectionStringBuilder = new FbConnectionStringBuilder(connectionString);
        }

        public object this[string propertyName]
        {
            get { return connectionStringBuilder[propertyName]; }
            set { connectionStringBuilder[propertyName] = value; }
        }

        public bool Contains(string propertyName)
        {
            return connectionStringBuilder.ContainsKey(propertyName);
        }

        
    }
}
