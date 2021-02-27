using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IStoreDatabaseDriver
    {
        void Configure(StoreDatabaseDriverSettings settings);

        // DDL
        bool TableExists(string schema, string tableName);
        void CreateTable<T>(string schema, string tableName = null);
        void CreateTable(string schema, StoreDefinition table);
        void CreateSchema(string schemaName);
        void RenameTable(string schemaName, string tableName, string newValue);
        void DeleteTable(string schemaName, string tableName);
        void AddColumn(string schemaName, string tableName, StoreProperty column);
        void DeleteColumn(string schemaName, string tableName, string columnName);
        void RenameColumn(string schemaName, string tableName, string columnName, string newValue);
        void AlterColumn(string schemaName, string tableName, string columnName, StoreProperty column);

        // CRUD
        IList Get(string schema, Type entityType);
        IList Find(string schema, Type entityType, object pkValue);
        IList Find(string schema, Type entityType, string filterColumn, object filterValue);
        IEnumerable<T> Find<T>(string schema, string tableName, string filterColumn, object filterValue);
        IEnumerable<T> Find<T>(string schema, string filterColumn, object filterValue);
        T Find<T>(string schema, object pkValue);
        long Insert(string schema, object record);
        long Insert(string schema, object record, string tableName);
        long Insert(string schema, object record, string idValue, string tableName);
        void Update(string schema, object record);
        void Delete(string schema, string tableName, object pkValue);
        void Delete(string schema, object record);
    }

    public class StoreDatabaseDriverSettings
    {
        public string ConnectionString { get; set; }
    }
}
