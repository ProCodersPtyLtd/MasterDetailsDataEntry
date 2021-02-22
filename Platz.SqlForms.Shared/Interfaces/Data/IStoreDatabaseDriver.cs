using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IStoreDatabaseDriver
    {
        void Configure(StoreDatabaseDriverSettings settings);
        bool TableExists(string schema, string tableName);
        void CreateTable<T>(string schema, string tableName = null);
        IEnumerable<T> Find<T>(string schema, string tableName, string filterColumn, object filterValue);
        IEnumerable<T> Find<T>(string schema, string filterColumn, object filterValue);
        T Find<T>(string schema, object pkValue);
    }

    public class StoreDatabaseDriverSettings
    {
        public string ConnectionString { get; set; }
    }
}
