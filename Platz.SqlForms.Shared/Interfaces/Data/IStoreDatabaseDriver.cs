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
        void CreateTable(string schema, StoreDefinition table);
        IEnumerable<T> Find<T>(string schema, string tableName, string filterColumn, object filterValue);
        IEnumerable<T> Find<T>(string schema, string filterColumn, object filterValue);
        T Find<T>(string schema, object pkValue);
        long Insert(string schema, object record);
        long Insert(string schema, object record, string tableName);
        void CreateSchema(string schemaName);
        void RenameTable(string schemaName, string tableName, string newValue);
        void DeleteTable(string schemaName, string tableName);
        void AddColumn(string schemaName, string tableName, StoreProperty column);
        void DeleteColumn(string schemaName, string tableName, string columnName);
        void RenameColumn(string schemaName, string tableName, string columnName, string newValue);
        void AlterColumn(string schemaName, string tableName, string columnName, StoreProperty column);
    }

    public class StoreDatabaseDriverSettings
    {
        public string ConnectionString { get; set; }
    }
}
