using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DataContextBase : IDisposable
    {
        private List<Type> _tables;
        private string _schema;
        protected string _connectionString;
        protected string _connectionStringConfigKey;
        protected IStoreDatabaseDriver _db;

        public DataContextBase() 
            : this (null, "DefaultConnection")
        {
        }

        public DataContextBase(string connectionString, string connectionStringConfigKey) 
        {
            _connectionString = connectionString;
            _connectionStringConfigKey = connectionStringConfigKey;

            if (string.IsNullOrEmpty(_connectionString))
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
                _connectionString = configuration.GetConnectionString(_connectionStringConfigKey);
            }

            var t = this.GetType();
            var settings = new DataContextSettings();
            Configure(settings);
            _tables = settings.GetTables().ToList();
            _schema = settings.GetSchemaName();

            _db = Activator.CreateInstance(settings.GetDriverType()) as IStoreDatabaseDriver;
            _db.Configure(new StoreDatabaseDriverSettings { ConnectionString = _connectionString });

        }

        public List<T> ExecuteQuery<T>(string sql, params object[] ps) 
        {
            var list = _db.ExecuteQueryParams(sql, typeof(T), ps);
            var result = list.Cast<T>().ToList();
            return result;
        }

        public IList ExecuteQuery(string sql, Type returnType, params object[] ps)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyInfo> FindPrimaryKey(Type type)
        {
            // Assumption first property is always PK
            var pk = type.GetProperties().First();
            return new PropertyInfo[] { pk };
        }

        protected abstract void Configure(DataContextSettings tables);

        public IList Get(Type entityType)
        {
            ValidateEntityType(entityType);
            return _db.Get(_schema, entityType);
        }

        /// <summary>
        /// Find by key
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="filterColumn"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IList Find(Type entityType, string filterColumn, object value)
        {
            ValidateEntityType(entityType);
            return _db.Find(_schema, entityType, filterColumn, value);
        }

        /// <summary>
        /// Find by primary key
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        public object Find(Type entityType, object pkValue)
        {
            ValidateEntityType(entityType);
            return _db.Find(_schema, entityType, pkValue);
        }

        public long Insert(object record)
        {
            var entityType = record.GetType();
            ValidateEntityType(entityType);
            return _db.Insert(_schema, record);
        }

        public void Update(object record)
        {
            var entityType = record.GetType();
            ValidateEntityType(entityType);
            _db.Update(_schema, record);
        }

        public void Delete(object record)
        {
            var entityType = record.GetType();
            ValidateEntityType(entityType);
            _db.Delete(_schema, record);
        }

        public void Delete(Type entityType, object pkValue)
        {
            ValidateEntityType(entityType);
            _db.Delete(_schema, entityType.Name, pkValue);
        }

        private void ValidateEntityType(Type entityType)
        {
            if (!_tables.Contains(entityType))
            {
                throw new DataContextException($"Type '{entityType.Name}' is not registered as a table.");
            }
        }

        public void Dispose()
        {
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
