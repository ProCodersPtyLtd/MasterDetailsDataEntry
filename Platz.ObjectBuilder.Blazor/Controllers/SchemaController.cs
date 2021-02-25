using Platz.ObjectBuilder.Blazor.Controllers.Validation;
using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers
{
    public interface ISchemaMvvm
    {
        string SelectedEditTab { get; }
        bool UseBigIntId { get; }
        DesignTable SelectedTable { get; }

        void SelectSchemaTab();
        void SelectLogTab();
        void SelectTableTab(int row);
        void SelectTable(DesignTable table);
    }

    public interface ISchemaController : ISchemaMvvm
    {
        DesignSchema Schema { get; }
        SchemaControllerParameters Parameters { get; }
        string FullStoreDataPath { get; set; }
        int ListSelectedRow { get; set; }

        void SetParameters(SchemaControllerParameters parameters);
        //void LoadSchema();
        void NewSchema();

        DesignTable CreateTable();
        void DeleteTable(DesignTable table);
        void UpdateLog(DesignOperation operation, DesignTable table = null, DesignColumn column = null);
        string GetDesignLog();

        // Files
        List<RuleValidationResult> ValidationResults { get; }
        string Errors { get; set; }
        void SaveSchema(string path);
        void SaveMigrations(string path);
        void Validate();
        List<string> GetFileList(string path);
        void LoadFromFile(string path, string fileName);
        bool FileExists(string path);
        string GenerateFileName(string path);

        // work with DB
        void ApplyMigrations();
    }

    public class SchemaControllerParameters
    {
        public string StoreDataPath { get; set; }

        public string Namespace { get; set; }

        public string DataService { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SchemaController : ISchemaController
    {
        private const string INITIAL_VERSION = "1.0";
        private const string SchemaFileSuffix = ".schema.json";
        private const string SchemaMigrationsFileSuffix = ".schema.migrations.json";

        private readonly SchemaTableDesignController _tableController;
        private readonly IStoreSchemaStorage _schemaStorage;
        private readonly IDataMigrationManager _migrationManager;

        private List<DesignLogRecord> _designRecords = new List<DesignLogRecord>();

        public string FullStoreDataPath { get; set; }
        public SchemaControllerParameters Parameters { get; protected set; }
        public DesignSchema Schema { get; protected set; } 
        public int ListSelectedRow { get; set; }
        public List<RuleValidationResult> ValidationResults { get; set; } = new List<RuleValidationResult>();
        public string Errors { get; set; }


        private Dictionary<Guid, object> _objectClones = new Dictionary<Guid, object>();

        public SchemaController(IStoreSchemaStorage schemaStorage, SchemaTableDesignController tableController, IDataMigrationManager migrationManager)
        {
            _schemaStorage = schemaStorage;
            _tableController = tableController;
            _migrationManager = migrationManager;
        }

        private T FindClone<T>(Guid id) where T: class
        {
            if (_objectClones.ContainsKey(id))
            {
                return (T)_objectClones[id];
            }

            return null;
        }

        #region log
        public void ClearLog()
        {
            _designRecords.Clear();
        }

        /// <summary>
        /// Keeps log that allows to record migrations and undo/redo
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        public void UpdateLog(DesignOperation operation, DesignTable table = null, DesignColumn column = null)
        {
            string old = null;
            string newValue = null;

            switch (operation)
            {
                case DesignOperation.SetTableName:
                    newValue = table.Name;
                    old = FindClone<DesignTable>(table.Id)?.Name;
                    break;

                case DesignOperation.SetColumnName:
                    newValue = column.Name;
                    old = FindClone<DesignColumn>(column.Id)?.Name;
                    break;

                case DesignOperation.SetColumnNullable:
                    newValue = column.Nullable.ToString();
                    old = FindClone<DesignColumn>(column.Id)?.Nullable.ToString();
                    break;

                case DesignOperation.SetColumnType:
                    newValue = column.Type;
                    old = FindClone<DesignColumn>(column.Id)?.Type;
                    break;

                case DesignOperation.SetColumnReference:
                    newValue = column.Reference;
                    old = FindClone<DesignColumn>(column.Id)?.Reference;
                    break;
            }

            _designRecords.Add(new DesignLogRecord { Operation = operation, TableName = table?.Name, ColumnName = column?.Name, OldValue = old, NewValue = newValue });

            // save clones
            if (table != null)
            {
                _objectClones[table.Id] = table.GetCopy(table.Id);
            }

            if (column != null)
            {
                _objectClones[column.Id] = column.GetCopy(column.Id);
            }
        }

        public string GetDesignLog()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _designRecords.Count; i++)
            {
                var r = _designRecords[i];
                sb.Append($"{i} {Enum.GetName(r.Operation)}");

                if (r.TableName != null)
                {
                    sb.Append($" Table '{r.TableName}'");
                }

                if (r.ColumnName != null)
                {
                    sb.Append($" Column '{r.ColumnName}'");
                }

                if (r.OldValue != null)
                {
                    sb.Append($" from '{r.OldValue}'");
                }

                if (r.NewValue != null)
                {
                    sb.Append($" to '{r.NewValue}'");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        #endregion

        #region Mvvm

        public string SelectedEditTab { get; protected set; } = "Schema";
        public bool UseBigIntId { get; protected set; }
        public DesignTable SelectedTable { get; protected set; }

        public void SelectTable(DesignTable table)
        {
            SelectedTable = table;
            _tableController.Load(Schema, SelectedTable);
            _tableController.Update();
        }

        public void SelectSchemaTab()
        {
            SelectedEditTab = "Schema";
            ListSelectedRow = 0;
        }

        public void SelectLogTab()
        {
            SelectedEditTab = "Log";
        }

        public void SelectTableTab(int row)
        {
            SelectedEditTab = "Table";

            if (SelectedTable != null)
            {
                _tableController.Update();
            }
        }

        #endregion

        public DesignTable CreateTable()
        {
            var table = _tableController.CreateTable(Schema);
            _objectClones[table.Id] = table.GetCopy(table.Id);
            Schema.Tables.Add(table);
            _tableController.Changed();
            UpdateLog(DesignOperation.CreateTable, table);
            return table;
        }

        public void DeleteTable(DesignTable table)
        {
            Schema.Tables.Remove(table);
            SelectedTable = null;
            SelectSchemaTab();
            UpdateLog(DesignOperation.DeleteTable, table);
        }

        public void SetParameters(SchemaControllerParameters parameters)
        {
            Parameters = parameters;
        }

        //public void LoadSchema()
        //{
        //}

        public void NewSchema()
        {
            Schema = new DesignSchema { Name = "New Schema", Version = "1.0", DataContextName = Parameters.DataService, Changed = true, IsNew = true };
            SelectedTable = null;
            ClearLog();
            UpdateLog(DesignOperation.CreateSchema);
        }

        public void SaveSchema(string path)
        {
            var fileName = Path.Combine(path, GenerateFileName(path));
            var parameters = new StorageParameters { FileName = fileName };
            var schema = DesignSchemaConvert.ToStoreSchema(Schema);
            _schemaStorage.SaveSchema(schema, parameters);

            // set all changed properties
            ClearChanged();

            // clear logs ?
        }

        private void ClearChanged()
        {
            Schema.Changed = false;
            Schema.Tables.ForEach(t => 
            { 
                t.Changed = false;
                var last = t.Columns.Last();

                if (!last.IsEmpty())
                {
                    t.Columns.Add(new DesignColumn { });
                }
            });

            SelectedTable = null;
        }

        public void SaveMigrations(string path)
        {
            StoreSchemaMigrations package = GenerateMigrations(Schema, _designRecords);
            var fileName = Path.Combine(path, GenerateMigrationFileName(path));
            var parameters = new StorageParameters { FileName = fileName };
            var schema = DesignSchemaConvert.ToStoreSchema(Schema);
            _schemaStorage.SaveMigration(package, parameters);
        }

        private static StoreSchemaMigrations GenerateMigrations(DesignSchema schema, List<DesignLogRecord> log)
        {
            if (schema.Version == INITIAL_VERSION)
            {
                var package = new StoreSchemaMigrations { SchemaName = schema.Name, Migrations = new StoreMigration[] { GenerateInitialMigration(schema) } };
                return package;
            }
            else
            {
                //return GenerateIncrementalMigration(schema, log);
                throw new NotImplementedException();
            }
        }

        private static StoreMigration GenerateInitialMigration(DesignSchema schema)
        {
            if (schema.Changed)
            {
                schema.VersionKey = Guid.NewGuid();
            }

            var mp = new StoreMigration { Version = schema.Version, VersionKey = schema.VersionKey };
            var cmd = new List<MigrationCommand>();

            cmd.Add(new MigrationCommand { Operation = MigrationOperation.CreateSchema, SchemaName = schema.Name });

            foreach (var t in schema.Tables)
            {
                var tm = new MigrationCommand { Operation = MigrationOperation.CreateTable, SchemaName = schema.Name, Table = DesignSchemaConvert.ToStoreDefinition(t) };
                tm.OperationCode = Enum.GetName(tm.Operation);
                cmd.Add(tm);
            }

            mp.Commands = cmd.ToArray();
            return mp;
        }

        private static StoreMigration GenerateIncrementalMigration(DesignSchema schema, List<DesignLogRecord> designRecords)
        {
            throw new NotImplementedException();
        }


        public void Validate()
        {
            ValidationResults.Clear();
            // ToDo:
        }

        public List<string> GetFileList(string path)
        {
            //return (new string[] { "a", "b", "c" }).ToList();
            var result = _schemaStorage.GetFileNames(new StorageParameters { Path = path });
            result = result.Where(f => f.Contains(SchemaFileSuffix) && !f.Contains(SchemaMigrationsFileSuffix)).ToList();
            return result;
        }

        public void LoadFromFile(string path, string fileName)
        {
            var schema = _schemaStorage.LoadSchema(new StorageParameters { FileName = fileName, Path = path });
            Schema = DesignSchemaConvert.FromStoreSchema(schema);
            ClearChanged();
            ClearLog();
        }

        public bool FileExists(string path)
        {
            return _schemaStorage.FileExists(new StorageParameters { Path = path, FileName = GenerateFileName(path) });
        }

        public string GenerateFileName(string path)
        {
            return Path.Combine(path, $"{Schema.Name}{SchemaFileSuffix}");
        }

        public string GenerateMigrationFileName(string path)
        {
            return Path.Combine(path, $"{Schema.Name}{SchemaMigrationsFileSuffix}");
        }

        public void ApplyMigrations()
        {
            var package = GenerateMigrations(Schema, _designRecords);
            _migrationManager.Configure(new StoreDatabaseDriverSettings { ConnectionString = Parameters.ConnectionString });
            _migrationManager.ApplyMigrations(package);
            throw new NotImplementedException();
        }
    }
}
