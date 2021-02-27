using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DataContextBase
    {
        private List<Type> _tables;
        private string _schema;
        protected string _connectionString;
        protected string _connectionStringConfigKey;
        protected IStoreDatabaseDriver _db;

        public DataContextBase()
        {
            var t = this.GetType();
            var collection = new DataContextSettings();
            Configure(collection);
            _tables = collection.GetTables().ToList();
            _schema = collection.GetSchemaName();

            _db = Activator.CreateInstance(collection.GetDriverType()) as IStoreDatabaseDriver;
            _db.Configure(new StoreDatabaseDriverSettings { ConnectionString = _connectionString });
        }

        public DataContextBase(string connectionString) :
            this()
        {
            _connectionString = connectionString;
        }

        protected abstract void Configure(DataContextSettings tables);

        public IList Get(Type entityType)
        {
            ValidateEntityType(entityType);
            return _db.Get(_schema, entityType);
        }

        private void ValidateEntityType(Type entityType)
        {
            if (!_tables.Contains(entityType))
            {
                throw new DataContextException($"Type '{entityType.Name}' is not registered as a table.");
            }
        }
    }

    public class DataContextSettings
    {
        private List<Type> _tables = new List<Type>();
        private string _schemaName;
        private Type _driver;

        public DataContextSettings AddTable<T>()
        {
            _tables.Add(typeof(T));
            return this;
        }

        public void SetSchema(string schemaName)
        {
            _schemaName = schemaName;
        }

        public IEnumerable<Type> GetTables()
        {
            return _tables;
        }

        public string GetSchemaName()
        {
            return _schemaName;
        }

        public void SetDriver<T>() where T : class, IStoreDatabaseDriver
        {
            _driver = typeof(T);
        }

        public Type GetDriverType()
        {
            return _driver;
        }
    }

    public class DataContextException : Exception
    {
        public DataContextException() : base()
        {
        }

        public DataContextException(string message) : base(message)
        {
        }
    }
}
