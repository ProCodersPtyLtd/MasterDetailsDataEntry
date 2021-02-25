using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Platz.SqlForms
{
    /// <summary>
    /// 1. First property of the entity is always Primary Key
    /// 2. Second column of each table is json contaning entity object
    /// </summary>
    public class SqlJsonStoreDatabaseDriver : IStoreDatabaseDriver
    {
        public const string DATA_COLUMN = "data";
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
            var sql = @$"
IF NOT EXISTS ( SELECT  *
                FROM    sys.schemas
                WHERE   name = N'{schemaName}' )
    EXEC('CREATE SCHEMA [{schemaName}]');
";

            ExecuteNonQuery(sql);
        }

        public void CreateTable<T>(string schema, string tableName = null)
        {
            var table = GetTableFromType(typeof(T));
            table.Name = tableName ?? table.Name;
            CreateTable(schema, table);
        }

        private StoreDefinition GetTableFromType(Type entityType)
        {
            var d = new StoreDefinition { Name = entityType.Name };
            int i = 0;

            d.Properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new StoreProperty { Name = p.Name, Type = p.PropertyType.Name, Pk = i == 0, Order = i++ })
                .ToDictionary(p => p.Name, p => p);

            return d;
        }

        public void CreateTable(string schema, StoreDefinition table)
        {
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var pkColumn = GetPkColumn(columns[0]);
            
            var sql = @$"
CREATE TABLE [{schema}].[{table.Name}] ({pkColumn}, {DATA_COLUMN} nvarchar(max));

ALTER TABLE [{schema}].[{table.Name}]
    ADD CONSTRAINT [{schema}_{table.Name}_JSON]
                   CHECK (ISJSON({DATA_COLUMN})=1);
";

            ExecuteNonQuery(sql);
        }

        private void ExecuteNonQuery(string sql)
        {
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    var e = cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteNonQueryP1(string sql, string p1)
        {
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("p1", p1);
                    var e = cmd.ExecuteNonQuery();
                }
            }
        }

        private string GetPkColumn(StoreProperty pk)
        {
            switch (pk.Type)
            {
                case "int":
                    return $"{pk.Name} bigint IDENTITY(1,1) PRIMARY KEY";
                case "Guid":
                    return $"{pk.Name} uniqueidentifier PRIMARY KEY";
                default:
                    return $"{pk.Name} varchar(32) PRIMARY KEY";
            }
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
            var sql = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @p1 AND  TABLE_NAME = @p2";

            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("p1", schema);
                    cmd.Parameters.AddWithValue("p2", tableName);
                    var e = cmd.ExecuteScalar();

                    if (e != null && Convert.ToInt32(e) == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        public long Insert(string schema, object record, string idValue, string tableName)
        {
            var type = record.GetType();
            var table = GetTableFromType(type);
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            string json = System.Text.Json.JsonSerializer.Serialize(record, type);
            string sql;

            switch (columns[0].Type)
            {
                case "int":
                    throw new NotImplementedException();
                case "guid":
                    throw new NotImplementedException();
                default:
                    sql = SqlScriptHelper.InsertJsonTablePkString(tableName, schema, idValue, "p1");
                    break;
            }

            ExecuteNonQueryP1(sql, json);
            return 0;
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

