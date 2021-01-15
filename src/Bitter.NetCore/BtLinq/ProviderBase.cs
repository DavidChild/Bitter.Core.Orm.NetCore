using System;
using System.Linq;
using System.Reflection;
using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    public abstract class ProviderBase
    {
        private readonly DatabaseType _databaseType = dc.dbconn(string.Empty).Reader.DatabaseType;
        private Type _type;


        public DatabaseType DatabaseType
        {
            get { return _databaseType; }
        }


        public virtual ObjectPropertyConvertType ObjectPropertyConvertType
        {
            get { return ObjectPropertyConvertType.Cast; }
        }

        public abstract IModelOprator CreateEntityOperator();


        public ParserBase CreateParser()
        {
            return new Parser();
        }


        public BuilderFactory CreateSqlBuilderFactory()
        {
            if (string.IsNullOrWhiteSpace(ConfigManager.SqlBuilder))
            {
                return new BuilderFactory(this);
            }
            if (_type == null)
            {
                char[] separator = {','};
                var assInfos = ConfigManager.SqlBuilder.Split(separator);
                _type = Assembly.Load(assInfos[1]).GetTypes().FirstOrDefault(x => x.FullName == assInfos[0]);
            }
            object[] parameters = {this};
            return
                (BuilderFactory) ExpressionReflector.CreateInstance(_type, ObjectPropertyConvertType.Cast, parameters);
        }
    }
}