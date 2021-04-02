using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDataMigrationManager
    {
        void Configure(StoreDatabaseDriverSettings settings);
        void ApplyMigrations(string fileName);
        void ApplyMigrations(StoreSchemaMigrations package);
        void DropSchemaWithObjects(string schemaName);
        bool MigrationApplied(string schema, StoreMigration migration);
        string MigrationToString(StoreMigration migration);
    }

    public class DataMigrationException : Exception
    {
        public DataMigrationException() : base()
        {
        }

        public DataMigrationException(string message) : base(message)
        {
        }
    }
}
