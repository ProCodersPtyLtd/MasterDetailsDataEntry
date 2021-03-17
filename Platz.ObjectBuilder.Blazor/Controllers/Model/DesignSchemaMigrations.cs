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
        public List<DesignMigration> Migrations { get; set; } = new List<DesignMigration>();

        public static DesignSchemaMigrations FromStoreMigrations(StoreSchemaMigrations src)
        {
            var result = new DesignSchemaMigrations();
            result.Migrations = src.Migrations.Select(s => new DesignMigration 
            { 
                Migration = s,
                StatusText = Enum.GetName(typeof(MigrationStatus), s.Status),
                VersionText = s.Version == "1.0" ? "Initial 1.0": $"{s.FromVersion} -> {s.Version}",
            }).ToList();

            return result;
        }
    }

    public class DesignMigration
    {
        public StoreMigration Migration { get; set; }
        public string VersionText { get; set; }
        public string StatusText { get; set; }

        public bool IsDeleteEnabled { get { return Migration?.Status == MigrationStatus.Empty; } }
    }
}
