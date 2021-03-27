using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class DesignSchemaMigrations
    {
        public const string InitialVersionText = "Initial 1.0";
        public const string InitialVersion = "1.0";

        public List<DesignMigration> Migrations { get; set; } = new List<DesignMigration>();

        public StoreMigration[] GetStoreMigrations()
        {
            var result = Migrations.Select(s => s.Migration).ToArray();
            return result;
        }

        public static DesignSchemaMigrations FromStoreMigrations(StoreSchemaMigrations src)
        {
            var result = new DesignSchemaMigrations();
            result.Migrations = src.Migrations.Select(s => new DesignMigration(s)).ToList();
            return result;
        }
    }

    public class DesignMigration
    {
        public DesignMigration(StoreMigration s)
        {
            Migration = s;
            //StatusText = Enum.GetName(typeof(MigrationStatus), s.Status);
            //VersionText = s.Version == DesignSchemaMigrations.InitialVersion ? DesignSchemaMigrations.InitialVersionText : $"{s.FromVersion} -> {s.Version}";
        }

        public StoreMigration Migration { get; set; }
        public string VersionText 
        { 
            get 
            { 
                return Migration.Version == DesignSchemaMigrations.InitialVersion ? DesignSchemaMigrations.InitialVersionText : $"{Migration.FromVersion} -> {Migration.Version}";
            } 
        }

        public string StatusText
        {
            get
            {
                return Enum.GetName(typeof(MigrationStatus), Migration.Status);
            }
        }

        public bool IsDeleteEnabled { get { return Migration?.Status == MigrationStatus.Empty && VersionText != DesignSchemaMigrations.InitialVersionText; } }
    }
}
