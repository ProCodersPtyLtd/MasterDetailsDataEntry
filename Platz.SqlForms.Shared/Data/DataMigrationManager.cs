using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Platz.SqlForms
{
    public class DataMigrationManager : IDataMigrationManager
    {
        private const string VERSION_TABLE = "_version";
        private const string VERSION_ID = "Version";
        private const string INITIAL_VERSION = "1.0";
        private readonly IStoreDatabaseDriver _storeDatabaseDriver;

        public DataMigrationManager(IStoreDatabaseDriver storeDatabaseDriver)
        {
            _storeDatabaseDriver = storeDatabaseDriver;
        }

        public void Configure(StoreDatabaseDriverSettings settings)
        {
            _storeDatabaseDriver.Configure(settings);
        }

        public void DropSchemaWithObjects(string schemaName)
        {
            _storeDatabaseDriver.DropSchemaWithObjects(schemaName);
        }

        public void ApplyMigrations(string fileName)
        {
            using var r = new StreamReader(fileName);
            var json = r.ReadToEnd();
            var package = JsonSerializer.Deserialize<StoreSchemaMigrations>(json);
            ApplyMigrations(package);
        }

        public void ApplyMigrations(StoreSchemaMigrations package)
        {
            foreach (var migration in package.Migrations)
            {
                if (MigrationApplied(package.SchemaName, migration))
                {
                    continue;
                }

                ApplyMigration(package.SchemaName, migration);
            }
        }

        public void ApplyMigration(string schema, StoreMigration migration)
        {
            int nextIndex = 0;

            try
            {
                // try read version - table maybe doesn't exist
                var lastVersionRecord = _storeDatabaseDriver.Find<MigrationVersionEntity>(schema, VERSION_TABLE, VERSION_ID, migration.Version);

                if (lastVersionRecord.Any())
                {
                    nextIndex = lastVersionRecord.First().NextIndex;
                }
            }
            catch { }

            // skip migrations that already applied;
            var commands = migration.Commands.Skip(nextIndex);

            foreach (var command in commands)
            {
                nextIndex++;

                switch (command.Operation)
                {
                    // ToDo: add foreign key constraints 
                    case MigrationOperation.CreateSchema:
                        _storeDatabaseDriver.CreateSchema(command.SchemaName);
                        _storeDatabaseDriver.CreateTable<MigrationVersionEntity>(schema, VERSION_TABLE);
                        // this operation is not considered as object operation
                        //nextIndex--;
                        break;
                    case MigrationOperation.AlterSchemaName:
                        // https://www.sqlservercentral.com/articles/renaming-a-schema-in-sql-server
                        throw new NotImplementedException("SQL Server doesn't support simple schema rename. ");
                    case MigrationOperation.CreateTable:
                        _storeDatabaseDriver.CreateTable(command.SchemaName, command.Table);
                        break;

                    // V1.0+ migrations
                    case MigrationOperation.DeleteTable:
                        _storeDatabaseDriver.DeleteTable(command.SchemaName, command.TableName);
                        break;
                    case MigrationOperation.AlterTableName:
                        _storeDatabaseDriver.RenameTable(command.SchemaName, command.TableName, command.NewValue);
                        break;

                    case MigrationOperation.AddColumn:
                        _storeDatabaseDriver.AddColumn(command.SchemaName, command.TableName, command.Column);
                        break;
                    case MigrationOperation.DeleteColumn:
                        _storeDatabaseDriver.DeleteColumn(command.SchemaName, command.TableName, command.ColumnName, command.Column);
                        break;
                    case MigrationOperation.AlterColumnName:
                        _storeDatabaseDriver.RenameColumn(command.SchemaName, command.TableName, command.ColumnName, command.NewValue);
                        break;
                    case MigrationOperation.AlterColumn:
                        _storeDatabaseDriver.AlterColumn(command.SchemaName, command.TableName, command.Column);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // save to version table
            var vesionRecord = new MigrationVersionEntity { 
                Applied = DateTime.UtcNow, Version = migration.Version, VersionKey = migration.VersionKey, NextIndex = nextIndex };

            var existingRecord = _storeDatabaseDriver.Find<MigrationVersionEntity>(schema, VERSION_TABLE, VERSION_ID, migration.Version);

            if (existingRecord.Any())
            {
                _storeDatabaseDriver.Update(schema, vesionRecord, VERSION_TABLE);
            }
            else
            {
                _storeDatabaseDriver.Insert(schema, vesionRecord, migration.Version, VERSION_TABLE);
            }
        }

        public bool MigrationApplied(string schema, StoreMigration migration)
        {
            if (!_storeDatabaseDriver.TableExists(schema, VERSION_TABLE))
            {
                //_storeDatabaseDriver.CreateTable<MigrationVersionEntity>(schema, VERSION_TABLE);
                return false;
            }

            var versionRecord = _storeDatabaseDriver.Find<MigrationVersionEntity>(schema, VERSION_TABLE, VERSION_ID, migration.Version);

            if (versionRecord.Any())
            {
                if (migration.Version == INITIAL_VERSION && versionRecord.First().VersionKey != migration.VersionKey)
                {
                    throw new DataMigrationException(
                        $"Incompatible initial migration already applied, expected key is {migration.VersionKey} but key in the database is {versionRecord.First().VersionKey}. Clear target database and try again.");
                }

                // edititng incrementatl migration can be partially applied
                if (/*migration.Version != INITIAL_VERSION &&*/ migration.Commands.Length != versionRecord.First().NextIndex) 
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public string MigrationToString(StoreMigration migration)
        {
            var sb = new StringBuilder();

            foreach (var cmd in migration.Commands)
            {
                switch (cmd.Operation)
                {
                    // ToDo: add foreign key constraints 
                    case MigrationOperation.CreateTable:
                        GenerateTableScript(sb, cmd.Table);
                        break;
                    case MigrationOperation.AddColumn:
                        GenerateColumnScript(sb, cmd.TableName, cmd.Column, "CREATE");
                        break;
                    case MigrationOperation.AlterColumn:
                        GenerateColumnScript(sb, cmd.TableName, cmd.Column, "ALTER");
                        break;
                    case MigrationOperation.AlterColumnName:
                        GenerateColumnRenameScript(sb, cmd.TableName, cmd.ColumnName, cmd.NewValue);
                        break;
                    case MigrationOperation.CreateSchema:
                        sb.AppendLine($"CREATE SCHEMA ({cmd.SchemaName})");
                        break;
                    case MigrationOperation.AlterSchemaName:
                        sb.AppendLine($"ALTER SCHEMA {cmd.SchemaName} ({cmd.NewValue})");
                        break;

                    // V1.0+ migrations
                    case MigrationOperation.DeleteTable:
                        sb.AppendLine($"DELETE TABLE ({cmd.TableName})");
                        break;
                    case MigrationOperation.AlterTableName:
                        sb.AppendLine($"RENAME TABLE {cmd.TableName} ({cmd.NewValue})");
                        break;
                    case MigrationOperation.DeleteColumn:
                        sb.AppendLine($"DELETE COLUMN ({cmd.TableName}.{cmd.ColumnName})");
                        break;
                    default:
                        sb.AppendLine($"{cmd.OperationCode}");
                        break;
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void GenerateColumnRenameScript(StringBuilder sb, string tableName, string colName, string newValue)
        {
            sb.AppendLine($"RENAME COLUMN {tableName}.{colName} ({newValue})");
        }

        private void GenerateColumnScript(StringBuilder sb, string tableName, StoreProperty c, string op)
        {
            var nullable = c.Nullable ? "?" : "";
            var reference = "";
            var type = c.Type;

            if (c.Fk)
            {
                reference = $" FK {c.ForeignKeys[0].DefinitionName}.{c.ForeignKeys[0].PropertyName}";
                type = c.ForeignKeys[0].Type;
            }

            sb.AppendLine($"{op} COLUMN {tableName}.{c.Name} ({type}{nullable}{reference})");
        }

        private void GenerateTableScript(StringBuilder sb, StoreDefinition table)
        {
            sb.AppendLine($"CREATE TABLE {table.Name} (");

            foreach (var c in table.Properties.Values.OrderBy(v => v.Order))
            {
                var nullable = c.Nullable ? "?" : "";
                var reference = "";
                var type = c.Type;

                if (c.Fk)
                {
                    reference = $" FK {c.ForeignKeys[0].DefinitionName}.{c.ForeignKeys[0].PropertyName}";
                    type = c.ForeignKeys[0].Type;
                }

                sb.AppendLine($"    {c.Name} {type}{nullable}{reference},");
            }

            sb.AppendLine(")");
        }
    }

    public class MigrationVersionEntity
    {
        //public long Id { get; set; }
        public string Version { get; set; }
        public Guid VersionKey { get; set; }
        public DateTime Applied { get; set; }
        public int NextIndex { get; set; }
    }
}
