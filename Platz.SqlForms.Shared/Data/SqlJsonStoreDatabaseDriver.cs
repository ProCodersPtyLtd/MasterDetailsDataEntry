using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    /// <summary>
    /// 1. First property of the entity is always Primary Key
    /// 2. Second column of each table is json contaning entity object
    /// </summary>
    public class SqlJsonStoreDatabaseDriver : IStoreDatabaseDriver
    {
        private StoreDatabaseDriverSettings _settings;

        public void Configure(StoreDatabaseDriverSettings settings)
        {
            _settings = settings;
        }

        public void CreateTable<T>(string schema, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find<T>(string schema, string tableName, string filterColumn, object filterValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find<T>(string schema, string filterColumn, object filterValue)
        {
            throw new NotImplementedException();
        }

        public T Find<T>(string schema, object pkValue)
        {
            throw new NotImplementedException();
        }

        public bool TableExists(string schema, string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
