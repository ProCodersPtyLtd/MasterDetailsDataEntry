using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class SqlJsonDataMigrationManager : IDataMigrationManager
    {
        private const string VERSION_TABLE = "_version";
        private const string VERSION_ID = "Version";
        private readonly IStoreDatabaseDriver _storeDatabaseDriver;

        public SqlJsonDataMigrationManager(IStoreDatabaseDriver storeDatabaseDriver)
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
            }
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
