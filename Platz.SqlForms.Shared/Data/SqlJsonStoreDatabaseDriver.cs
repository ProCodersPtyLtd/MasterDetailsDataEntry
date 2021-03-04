using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Platz.SqlForms
{
    /// <summary>
    /// 1. First property of the entity is always Primary Key
    /// 2. Second column of each table is json contaning entity object
    /// </summary>
    public class SqlJsonStoreDatabaseDriver : IStoreDatabaseDriver
    {
        public const string DATA_COLUMN = "data";
        public const string SEQUENCE = "mainseq";
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
            var sql = SqlScriptHelper.CreateSchema(schemaName);

            // also create sequence for autoincrement
            sql += SqlScriptHelper.CreateSequence(SEQUENCE, schemaName);

            ExecuteNonQuery(sql);
        }

        public void CreateTable<T>(string schema, string tableName = null)
        {
            var table = GetTableFromType(typeof(T));
            table.Name = tableName ?? table.Name;
            CreateTable(schema, table);
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
        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public IList Get(string schema, Type entityType)
        {
            var table = GetTableFromType(entityType);
            var sql = $"SELECT * FROM {schema}.{table.Name}";
            var objects = ExecuteQueryP1(sql, entityType, null);
            return objects;
        }

        /// <summary>
        /// Find item by id
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="entityType"></param>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        public object Find(string schema, Type entityType, object pkValue)
        {
            var table = GetTableFromType(entityType);
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var sql = $"SELECT * FROM {schema}.{table.Name} WHERE {columns[0].Name}=@p1";
            var objects = ExecuteQueryP1(sql, entityType, pkValue);
            return objects.FirstOrDefault();
        }

        /// <summary>
        /// Find list of items by criteria
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="entityType"></param>
        /// <param name="filterColumn"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public IList Find(string schema, Type entityType, string filterColumn, object filterValue)
        {
            var table = GetTableFromType(entityType);
            var sql = $"SELECT * FROM {schema}.{table.Name} WHERE {filterColumn}=@p1";
            var objects = ExecuteQueryP1(sql, entityType, filterValue);
            return objects;
        }

        public IEnumerable<T> Find<T>(string schema, string tableName, string filterColumn, object filterValue)
        {
            //var table = GetTableFromType(typeof(T));
            //var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();

            // ToDo: assumption that filter columns always implemented as columns or virtual columns
            var sql = $"SELECT * FROM {schema}.{tableName} WHERE {filterColumn}=@p1";
            var objects = ExecuteQueryP1(sql, typeof(T), filterValue);
            var typedObjects = objects.OfType<T>();
            return typedObjects;
        }

        public IEnumerable<T> Find<T>(string schema, string filterColumn, object filterValue)
        {
            var table = GetTableFromType(typeof(T));
            return Find<T>(schema, table.Name, filterColumn, filterValue);
        }


        public T Find<T>(string schema, object pkValue)
        {
            var table = GetTableFromType(typeof(T));
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var result = Find<T>(schema, table.Name, columns[0].Name, pkValue);
            return result.FirstOrDefault();
        }

        public long Insert(string schema, object record)
        {
            var table = GetTableFromType(record.GetType());
            return Insert(schema, record, table.Name);
        }

        public long Insert(string schema, object record, string tableName)
        {
            var table = GetTableFromType(record.GetType());
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var pkName = columns[0].Name;
            object idValue = record.GetType().GetProperty(pkName).GetValue(record);
            string json = JsonSerializer.Serialize(record);
            string sql;

            switch (columns[0].Type)
            {
                case "int":
                case "Int64":
                case "Int32":
                case "bigint":
                case "long":
                    // autoincrement
                    sql = SqlScriptHelper.InsertJsonTableAutoIncrement(tableName, schema, SEQUENCE, pkName, "p1");
                    break;
                case "guid":
                    // guid autopopulate
                    Guid g = (Guid)idValue;

                    if (Guid.Empty == g)
                    {
                        g = Guid.NewGuid();
                        record.GetType().GetProperty(pkName).SetValue(record, g);
                    }

                    sql = SqlScriptHelper.InsertJsonTablePkString(tableName, schema, g.ToString(), "p1");
                    break;
                default:
                    sql = SqlScriptHelper.InsertJsonTablePkString(tableName, schema, idValue.ToString(), "p1");
                    break;
            }

            var result = ExecuteScalarP1(sql, json);
            
            if (result is int || result is long)
            {
                var i32 = Convert.ToInt32(result);
                record.GetType().GetProperty(pkName).SetValue(record, i32);
                return Convert.ToInt64(result);
            }    
            
            return 0;
        }

        public long Insert(string schema, object record, string idValue, string tableName)
        {
            var type = record.GetType();
            var table = GetTableFromType(type);
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var pkName = columns[0].Name;
            string json = JsonSerializer.Serialize(record, type);
            string sql;

            switch (columns[0].Type)
            {
                case "int":
                case "bigint":
                case "long":
                    sql = SqlScriptHelper.InsertJsonTableAutoIncrement(tableName, schema, SEQUENCE, pkName, "p1");
                    break;
                case "guid":
                    Guid g;
                    Guid.TryParse(idValue, out g);

                    if (Guid.Empty == g)
                    {
                        g = Guid.NewGuid();
                    }

                    sql = SqlScriptHelper.InsertJsonTablePkString(tableName, schema, g.ToString(), "p1");
                    break;
                default:
                    sql = SqlScriptHelper.InsertJsonTablePkString(tableName, schema, idValue, "p1");
                    break;
            }

            var result = ExecuteScalarP1(sql, json);

            if (result is int || result is long)
            {
                return Convert.ToInt64(result);
            }

            return 0;
        }

        public void Update(string schema, object record)
        {
            var table = GetTableFromType(record.GetType());
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var pkName = columns[0].Name;
            object idValue = record.GetType().GetProperty(pkName).GetValue(record);
            string json = JsonSerializer.Serialize(record);
            var sql = SqlScriptHelper.UpdateJsonTableWithParams(table.Name, schema);
            ExecuteNonQuery(sql, json, idValue);
        }

        public void Delete(string schema, string tableName, object pkValue)
        {
            var sql = SqlScriptHelper.DeleteJsonTableWithParams(tableName, schema);
            ExecuteNonQuery(sql, pkValue);
        }

        public void Delete(string schema, object record)
        {
            var table = GetTableFromType(record.GetType());
            var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();
            var pkName = columns[0].Name;
            object idValue = record.GetType().GetProperty(pkName).GetValue(record);
            Delete(schema, table.Name, idValue);
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

        private void ExecuteNonQuery(string sql, params object[] p)
        {
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    for (int i = 0; i < p.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"p{i+1}", p[i]);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<object> ExecuteQueryP1(string sql, Type entityType, object p1)
        {
            var result = new List<object>();
            //var table = GetTableFromType(entityType);
            //var columns = table.Properties.Values.OrderBy(p => p.Order).ToList();

            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    if (p1 != null)
                    {
                        cmd.Parameters.AddWithValue("p1", p1);
                    }

                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        var json = Convert.ToString(dr[DATA_COLUMN]);
                        var e = JsonSerializer.Deserialize(json, entityType);
                        result.Add(e);
                    }
                }
            }

            return result;
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

        private object ExecuteScalarP1(string sql, string p1)
        {
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("p1", p1);
                    var obj = cmd.ExecuteScalar();
                    return obj;
                }
            }
        }

        //private void ExecuteNonQueryP1(string sql, string p1)
        //{
        //    using (var conn = new SqlConnection(_settings.ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(sql, conn))
        //        {
        //            conn.Open();
        //            cmd.Parameters.AddWithValue("p1", p1);
        //            var e = cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        private string GetPkColumn(StoreProperty pk)
        {
            switch (pk.Type)
            {
                case "int":
                    //return $"{pk.Name} bigint IDENTITY(1,1) PRIMARY KEY";
                    // we use sequence for autoincrement
                    //return $"{pk.Name} bigint PRIMARY KEY";
                    return $"{pk.Name} int PRIMARY KEY";
                case "Guid":
                    return $"{pk.Name} uniqueidentifier PRIMARY KEY";
                default:
                    return $"{pk.Name} varchar(32) PRIMARY KEY";
            }
        }

    }
}

