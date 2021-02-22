using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class StoreSchemaMigrations
    {
        public string SchemaName { get; set; }
        public StoreMigration[] Migrations { get;set; }
    }

    public class StoreMigration
    {
        public string Version { get; set; }
        public Guid VersionKey { get; set; }
        public MigrationCommand[] Commands { get; set; }
    }

    public class MigrationCommand
    {
        public MigrationOperation Operation { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
 
        // When rename Table or Column name
        // When alter column - new column type
        public string NewValue { get; set; }

        // for Add Table
        public StoreDefinition Table { get; set; }

        // for Add column
        public StoreProperty Column { get; set; }
    }

    public enum MigrationOperation
    {
        CreateSchema = 1,
        //AlterSchemaName,

        CreateTable = 10,
        DeleteTable,
        AlterTableName,
        
        AddColumn = 20,
        DeleteColumn,
        AlterColumnName,
        AlterColumnType,
    }
}
