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

        List<DiagramTable> GetDiagramTables();
        List<TableLink> GetTableLinks();
        bool TryAddMigration(bool major, out string error);
        StoreSchemaMigrations GetCurrentMigrations();
    }

    public interface ISchemaController : ISchemaMvvm
    {
        DesignSchema Schema { get; }
        DesignSchemaMigrations SchemaMigrations { get; }
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
        void LoadMigrations(string path);
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
        public SchemaControllerParameters Parameters { get; private set; }
        public DesignSchema Schema { get; private set; } 
        public DesignSchemaMigrations SchemaMigrations { get; private set; }
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
        public void UpdateLog(DesignOperation op, DesignTable table = null, DesignColumn column = null)
        {
            var operation = op;
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

            if (op == DesignOperation.SetColumnName || op == DesignOperation.SetColumnNullable || op == DesignOperation.SetColumnType || op == DesignOperation.SetColumnReference)
            {
                if (FindClone<DesignColumn>(column.Id) == null)
                {
                    operation = DesignOperation.AddColumn;
                }
            }

            _designRecords.Add(
                new DesignLogRecord 
                { 
                    Operation = operation, TableName = table?.Name, TableId = table?.Id, ColumnName = column?.Name, ColumnId = column?.Id, 
                    OldValue = old, NewValue = newValue 
                });

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
            UpdateDiagramTables();
            return table;
        }

        public void DeleteTable(DesignTable table)
        {
            Schema.Tables.Remove(table);
            SelectedTable = null;
            SelectSchemaTab();
            UpdateDiagramTables();
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
            SchemaMigrations = new DesignSchemaMigrations();
            var nm = new StoreMigration { Version = DesignSchemaMigrations.InitialVersion, Created = DateTime.UtcNow };
            //SchemaMigrations.Migrations.Add(new DesignMigration { StatusText = "Empty", VersionText = "Initial 1.0" });
            SchemaMigrations.Migrations.Add(new DesignMigration(nm));
            SelectedTable = null;
            ClearLog();
            UpdateLog(DesignOperation.CreateSchema);

            UpdateDiagramTables();
        }

        public void LoadFromFile(string path, string fileName)
        {
            var schema = _schemaStorage.LoadSchema(new StorageParameters { FileName = fileName, Path = path });
            Schema = DesignSchemaConvert.FromStoreSchema(schema);
            ClearChanged();
            ClearLog();
            UpdateDiagramTables();
        }

        public void LoadMigrations(string path)
        {
            var fileName = Path.Combine(path, GenerateMigrationFileName(path));
            var package = _schemaStorage.LoadMigrations(new StorageParameters { FileName = fileName });
            SchemaMigrations = DesignSchemaMigrations.FromStoreMigrations(package);
        }

        public void SaveSchema(string path)
        {
            // Schema.VersionKey set during migration generation
            //if (Schema.Changed)
            //{
            //    Schema.VersionKey = Guid.NewGuid();
            //}

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

        public bool TryAddMigration(bool major, out string error)
        {
            error = null;

            if (Schema.Changed)
            {
                error = $"Schema should be saved before creating a new migration.";
                return false;
            }

            if (SchemaMigrations.Migrations.Any() && SchemaMigrations.Migrations.Last().StatusText == Enum.GetName(typeof(MigrationStatus), MigrationStatus.Empty))
            {
                error = $"You already have an empty migration that created recently.";
                return false;
            }

            if (SchemaMigrations.Migrations.Last().Migration == null)
            {
                error = $"Fatal error. Last migration is empty.";
                return false;
            }

            var last = SchemaMigrations.Migrations.Last();
            last.Migration.Status = MigrationStatus.Closed;
            var nm = new StoreMigration { Status = MigrationStatus.Empty, FromVersion = last.Migration.Version, Version = GetNextVersion(last.Migration.Version, major) };
            SchemaMigrations.Migrations.Add(new DesignMigration(nm));
            Schema.Version = nm.Version;

            return true;
        }

        private string GetNextVersion(string version, bool major)
        {
            var s = version.Split('.');
            var i = major ? 0 : 1;
            int v = int.Parse(s[i]);
            v++;
            s[i] = v.ToString();
            var result = string.Join('.', s);
            return result;
        }

        public StoreSchemaMigrations GetCurrentMigrations()
        {
            StoreSchemaMigrations package = GenerateMigrations(Schema, SchemaMigrations, _designRecords);
            return package;
        }

        public void SaveMigrations(string path)
        {
            StoreSchemaMigrations package = GenerateMigrations(Schema, SchemaMigrations, _designRecords);
            var fileName = Path.Combine(path, GenerateMigrationFileName(path));
            var parameters = new StorageParameters { FileName = fileName };
            var schema = DesignSchemaConvert.ToStoreSchema(Schema);
            _schemaStorage.SaveMigration(package, parameters);
        }

        private static StoreSchemaMigrations GenerateMigrations(DesignSchema schema, DesignSchemaMigrations src, List<DesignLogRecord> log)
        {
            if (schema.Version == INITIAL_VERSION)
            {
                var package = new StoreSchemaMigrations { SchemaName = schema.Name, Migrations = new StoreMigration[] { GenerateInitialMigration(schema) } };
                return package;
            }
            else
            {
                var package = new StoreSchemaMigrations { SchemaName = schema.Name, Migrations = src.GetStoreMigrations() };
                var lastMigration = package.Migrations[package.Migrations.Length - 1];
                var lastCommands = new List<MigrationCommand>(lastMigration.Commands ?? new MigrationCommand[0]);
                var newMigration = GenerateIncrementalMigration(schema, log);
                lastCommands.AddRange(newMigration.Commands);
                lastMigration.Commands = lastCommands.ToArray();
                return package;
            }
        }

        private static StoreMigration GenerateInitialMigration(DesignSchema schema)
        {
            if (schema.Changed)
            {
                schema.VersionKey = Guid.NewGuid();
            }

            var mp = new StoreMigration { Version = schema.Version, VersionKey = schema.VersionKey, Created = DateTime.UtcNow };
            mp.Status = schema.Tables.Any() ? MigrationStatus.Editing: MigrationStatus.Empty;
            var cmd = new List<MigrationCommand>();

            cmd.Add(new MigrationCommand { Operation = MigrationOperation.CreateSchema, SchemaName = schema.Name });

            foreach (var t in schema.Tables)
            {
                var tm = new MigrationCommand { Operation = MigrationOperation.CreateTable, SchemaName = schema.Name, Table = DesignSchemaConvert.ToStoreDefinition(schema, t) };
                tm.OperationCode = Enum.GetName(tm.Operation);
                cmd.Add(tm);
            }

            mp.Commands = cmd.ToArray();
            return mp;
        }

        private static StoreMigration GenerateIncrementalMigration(DesignSchema schema, List<DesignLogRecord> designRecords)
        {
            var result = new StoreMigration {  };
            var commands = new List<MigrationCommand>();
            var i = 0;
            var records = designRecords.ToList();

            while (i < records.Count)
            {
                switch (records[i].Operation)
                {
                    case DesignOperation.CreateTable:
                        ExtractCreateTable(schema, records[i], commands, records);
                        i--;
                        break;
                    case DesignOperation.SetSchemaName:
                        break;
                    case DesignOperation.SetTableName:
                        break;
                    case DesignOperation.DeleteTable:
                        break;
                    case DesignOperation.AddColumn:
                        ExtractAddColumn(schema, records[i], commands, records);
                        i--;
                        break;
                    case DesignOperation.SetColumnName:
                        break;
                    case DesignOperation.DeleteColumn:
                        break;
                    case DesignOperation.SetColumnType:
                        break;
                    case DesignOperation.SetColumnNullable:
                        break;
                    case DesignOperation.SetColumnReference:
                        break;
                }

                i++;
            }

            result.Commands = commands.ToArray();
            // log moved to migration
            designRecords.Clear();
            return result;
        }
        private static void ExtractAddColumn(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var cid = record.ColumnId;
            var tid = record.TableId;

            // because this is a new column we can use it from schema
            var table = schema.Tables.FirstOrDefault(t => t.Id == tid);

            // table can be deleted after that
            if (table != null)
            {
                var column = table.Columns.FirstOrDefault(c => c.Id == cid);

                // column can be deleted after that
                if (column != null)
                {
                    var tm = new MigrationCommand 
                    { 
                        Operation = MigrationOperation.AddColumn, SchemaName = schema.Name, TableName = table.Name, 
                        Column = DesignSchemaConvert.ToStoreProperty(schema, table, column) 
                    };
                    
                    tm.OperationCode = Enum.GetName(tm.Operation);
                    commands.Add(tm);
                }
            }

            // remove all logs about this column
            var colLogs = records.Where(r => r.ColumnId == cid).ToList();
            colLogs.ForEach(r => records.Remove(r));
        }

        private static void ExtractCreateTable(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var id = record.TableId;

            // because this is a new table we can use it from schema
            var table = schema.Tables.FirstOrDefault(t => t.Id == id);

            // table can be created and then deleted
            if (table != null)
            {
                var tm = new MigrationCommand 
                { 
                    Operation = MigrationOperation.CreateTable, SchemaName = schema.Name, 
                    Table = DesignSchemaConvert.ToStoreDefinition(schema, table) 
                };

                tm.OperationCode = Enum.GetName(tm.Operation);
                commands.Add(tm);
            }

            // remove all logs about this table
            var tableLogs = records.Where(r => r.TableId == id).ToList();
            tableLogs.ForEach(r => records.Remove(r));
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
            var package = GenerateMigrations(Schema, SchemaMigrations, _designRecords);
            _migrationManager.Configure(new StoreDatabaseDriverSettings { ConnectionString = Parameters.ConnectionString });
            _migrationManager.ApplyMigrations(package);
        }

        private List<DiagramTable> _diagramTables;

        public List<DiagramTable> GetDiagramTables()
        {
            if (_diagramTables == null)
            {
                UpdateDiagramTables();
            }

            return _diagramTables;
        }

        private void UpdateDiagramTables()
        {
            var tables = Schema.Tables.Select(t => new DiagramTable 
            { 
                Name = t.Name,
                Columns = t.Columns.Where(c => c.Name != null).Select(c => new Column 
                { 
                    Name = c.Name, Type = c.Type, IsPk = c.Pk, IsFk = c.Reference != null,
                    FkTable = c.TableReference, FkColumn = c.ColumnReference
                }).ToList()
            }).ToList();

            _diagramTables = tables;
        }

        public List<TableLink> GetTableLinks()
        {
            var result  = new List<TableLink>();
            var tables = GetDiagramTables();
            var order = 1;

            var links = tables.SelectMany(t => t.Columns, (t, c) => new { Table = t, Column = c })
                .Where(d => d.Column.IsFk).Select(d => new { d.Table.Name, d.Column.FkTable, d.Column.FkColumn });

            foreach (var link in links)
            {
                var forTable = tables.First(t => t.Name == link.Name);
                var forColumnIndex = forTable.Columns.FindIndex(c => c.FkTable == link.FkTable && c.FkColumn == link.FkColumn);
                //var forColumn = forTable.Columns.First(c => c.Name == link.FkColumn);
                var primTable = tables.First(t => t.Name == link.FkTable);
                //var primColumn = primTable.Columns.First();

                var tl = new TableLink 
                { 
                    PrimaryRefId = GenerateObjectId("table_primary", primTable.Id, 0),
                    ForeignRefId = GenerateObjectId("table_foreign", forTable.Id, forColumnIndex),
                    Order = order++
                };

                result.Add(tl);
            }

            return result;
        }

        private string GenerateObjectId(string prefix, int objId, int propId = 0)
        {
            var id = $"{prefix}_{objId}_{propId}";
            return id;
        }
    }
}
