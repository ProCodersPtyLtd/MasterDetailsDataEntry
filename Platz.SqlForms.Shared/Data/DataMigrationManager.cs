using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DataMigrationManager : IDataMigrationManager
    {
        private const string VERSION_TABLE = "_version";
        private const string VERSION_ID = "Version";
        private readonly IStoreDatabaseDriver _storeDatabaseDriver;

        public DataMigrationManager(IStoreDatabaseDriver storeDatabaseDriver)
        {
            _storeDatabaseDriver = storeDatabaseDriver;
        }

        public void Configure(StoreDatabaseDriverSettings settings)
        {
            _storeDatabaseDriver.Configure(settings);
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
            foreach (var command in migration.Commands)
            {
                switch (command.Operation)
                {
                    case MigrationOperation.CreateSchema:
                        _storeDatabaseDriver.CreateSchema(command.SchemaName);
                        break;
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
                        _storeDatabaseDriver.DeleteColumn(command.SchemaName, command.TableName, command.ColumnName);
                        break;
                    case MigrationOperation.AlterColumnName:
                        _storeDatabaseDriver.RenameColumn(command.SchemaName, command.TableName, command.ColumnName, command.NewValue);
                        break;
                    case MigrationOperation.AlterColumn:
                        _storeDatabaseDriver.AlterColumn(command.SchemaName, command.TableName, command.ColumnName, command.Column);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // save to version table
            var vesionRecord = new MigrationVersionEntity { Applied = DateTime.UtcNow, Version = migration.Version, VersionKey = migration.VersionKey };
            _storeDatabaseDriver.Insert(schema, vesionRecord, VERSION_TABLE);
        }

        public bool MigrationApplied(string schema, StoreMigration migration)
        {
            if (!_storeDatabaseDriver.TableExists(schema, VERSION_TABLE))
            {
                _storeDatabaseDriver.CreateTable<MigrationVersionEntity>(schema, VERSION_TABLE);
                return false;
            }

            var versionRecord = _storeDatabaseDriver.Find<MigrationVersionEntity>(schema, VERSION_TABLE, VERSION_ID, migration.Version);

            if (versionRecord.Any())
            {
                if (versionRecord.First().VersionKey != migration.VersionKey)
                {
                    throw new DataMigrationException($"Incompatible migration already applied, expected key is {migration.VersionKey} but key in the database is {versionRecord.First().VersionKey}");
                }

                return true;
            }

            return false;
        }
    }

    public class MigrationVersionEntity
    {
        //public long Id { get; set; }
        public string Version { get; set; }
        public Guid VersionKey { get; set; }
        public DateTime Applied { get; set; }
    }
}
