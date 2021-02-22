using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DataContextBase
    {
        private List<Type> _tables;
        protected string _connectionString;
        protected string _connectionStringConfigKey;

        public DataContextBase()
        {
            var t = this.GetType();
            var collection = new TableCollection();
            Configure(collection);
            _tables = collection.GetTables().ToList();
        }

        public DataContextBase(string connectionString) :
            this()
        {
            _connectionString = connectionString;
        }

        protected abstract void Configure(TableCollection tables);
    }

    public class TableCollection
    {
        private List<Type> _tables = new List<Type>();

        public TableCollection Add<T>()
        {
            _tables.Add(typeof(T));
            return this;
        }

        public IEnumerable<Type> GetTables()
        {
            return _tables;
        }
    }
}
