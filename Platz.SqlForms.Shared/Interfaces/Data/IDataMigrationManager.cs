using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDataMigrationManager
    {
        void Configure(StoreDatabaseDriverSettings settings);
        void ApplyMigrations(StoreSchemaMigrations package);
        bool MigrationApplied(string schema, StoreMigration migration);
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
