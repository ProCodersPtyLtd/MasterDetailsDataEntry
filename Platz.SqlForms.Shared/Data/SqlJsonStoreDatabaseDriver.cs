using Microsoft.EntityFrameworkCore;
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
            var ctx = new DummyContext(_settings.ConnectionString);
            ctx.Database.EnsureCreated();
        }

        #region DDL
        public void AddColumn(string schemaName, string tableName, StoreProperty column)
        {
            throw new NotImplementedException();
        }

        public void AlterColumn(string schemaName, string tableName, string columnName, StoreProperty column)
        {
            throw new NotImplementedException();
        }

        public void CreateSchema(string schemaName)
        {
            throw new NotImplementedException();
        }

        public void CreateTable<T>(string schema, string tableName = null)
        {
            throw new NotImplementedException();
        }

        public void CreateTable(string schema, StoreDefinition table)
        {
            throw new NotImplementedException();
        }

        public void DeleteColumn(string schemaName, string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        public void DeleteTable(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public void RenameColumn(string schemaName, string tableName, string columnName, string newValue)
        {
            throw new NotImplementedException();
        }

        public void RenameTable(string schemaName, string tableName, string newValue)
        {
            throw new NotImplementedException();
        }

        public bool TableExists(string schema, string tableName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CRUD
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

        public long Insert(string schema, object record)
        {
            throw new NotImplementedException();
        }

        public long Insert(string schema, object record, string tableName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Context
        public class DummyContext : DbContext
        {
            private readonly string _conn;
            public DummyContext(string conn)
            {
                _conn = conn;
            }

            //public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
            //{
            //}

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    //IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
                    //optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    optionsBuilder.UseSqlServer(_conn);
                }
            }
        }
        #endregion
    }
}

