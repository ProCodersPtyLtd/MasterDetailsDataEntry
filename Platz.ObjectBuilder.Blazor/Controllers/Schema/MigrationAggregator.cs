using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema
{
    public interface IMigrationAggregator
    {
        public StoreSchemaMigrations GenerateMigrations(DesignSchema schema, DesignSchemaMigrations src, List<DesignLogRecord> log);
    }

    public class MigrationAggregator : IMigrationAggregator
    {
        private const string INITIAL_VERSION = "1.0";

        public StoreSchemaMigrations GenerateMigrations(DesignSchema schema, DesignSchemaMigrations src, List<DesignLogRecord> log)
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
                // we clone it to make sure we don't modify SchemaMigrations data
                lastMigration = lastMigration.GetCopy() as StoreMigration;
                package.Migrations[package.Migrations.Length - 1] = lastMigration;
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
            mp.Status = schema.Tables.Any() ? MigrationStatus.Editing : MigrationStatus.Empty;
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
            var result = new StoreMigration { };
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
                        GenerateRenameSchemaOperation(schema, records[i], commands, records);
                        break;
                    case DesignOperation.SetTableName:
                        GenerateRenameTableOperation(schema, records[i], commands, records);
                        break;
                    case DesignOperation.DeleteTable:
                        GenerateDeleteTableOperation(schema, records[i], commands, records);
                        break;
                    case DesignOperation.AddColumn:
                        ExtractColumnOperation(schema, records[i], commands, records, MigrationOperation.AddColumn);
                        i--;
                        break;
                    case DesignOperation.DeleteColumn:
                        GenerateDeleteColumnOperation(schema, records[i], commands, records);
                        break;
                    case DesignOperation.SetColumnName:
                        GenerateRenameColumnOperation(schema, records[i], commands, records);
                        break;
                    case DesignOperation.SetColumnType:
                    case DesignOperation.SetColumnNullable:
                    case DesignOperation.SetColumnReference:
                        ExtractColumnOperation(schema, records[i], commands, records, MigrationOperation.AlterColumn);
                        i--;
                        break;
                }

                i++;
            }

            result.Commands = commands.ToArray();
            // log moved to migration
            //designRecords.Clear();
            return result;
        }

        private static void GenerateDeleteColumnOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var tm = new MigrationCommand
            {
                Operation = MigrationOperation.DeleteColumn,
                SchemaName = schema.Name,
                TableName = record.TableName,
                ColumnName = record.ColumnName
            };

            tm.OperationCode = Enum.GetName(tm.Operation);
            commands.Add(tm);
        }

        private static void GenerateDeleteTableOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var tm = new MigrationCommand
            {
                Operation = MigrationOperation.DeleteTable,
                SchemaName = schema.Name,
                TableName = record.TableName,
            };

            tm.OperationCode = Enum.GetName(tm.Operation);
            commands.Add(tm);
        }

        private static void GenerateRenameSchemaOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var tm = new MigrationCommand
            {
                Operation = MigrationOperation.AlterSchemaName,
                SchemaName = record.OldValue,
                NewValue = record.NewValue
            };

            tm.OperationCode = Enum.GetName(tm.Operation);
            commands.Add(tm);
        }

        private static void GenerateRenameTableOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var tid = record.TableId;
            var table = schema.Tables.FirstOrDefault(t => t.Id == tid);

            // table can be deleted after that
            //if (table != null)
            {
                var tm = new MigrationCommand
                {
                    Operation = MigrationOperation.AlterTableName,
                    SchemaName = schema.Name,
                    TableName = record.OldValue,
                    NewValue = record.NewValue
                };

                tm.OperationCode = Enum.GetName(tm.Operation);
                commands.Add(tm);
            }
        }

        private static void GenerateRenameColumnOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records)
        {
            var cid = record.ColumnId;
            var tid = record.TableId;
            var table = schema.Tables.FirstOrDefault(t => t.Id == tid);

            // table can be deleted after that
            if (table != null)
            {
                var column = table.Columns.FirstOrDefault(c => c.Id == cid);

                // column can be deleted after that
                //if (column != null)
                {
                    var tm = new MigrationCommand
                    {
                        Operation = MigrationOperation.AlterColumnName,
                        SchemaName = schema.Name,
                        TableName = table.Name,
                        ColumnName = record.OldValue,
                        NewValue = record.NewValue
                    };

                    tm.OperationCode = Enum.GetName(tm.Operation);
                    commands.Add(tm);
                }
            }
        }

        private static void ExtractColumnOperation(DesignSchema schema, DesignLogRecord record, List<MigrationCommand> commands, List<DesignLogRecord> records,
            MigrationOperation op)
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
                        Operation = op,
                        SchemaName = schema.Name,
                        TableName = table.Name,
                        Column = DesignSchemaConvert.ToStoreProperty(schema, table, column)
                    };

                    tm.OperationCode = Enum.GetName(tm.Operation);
                    commands.Add(tm);

                }
                else
                {
                    // if column created and deleted in this log time
                    if (records.Any(r => r.ColumnId == cid && r.Operation == DesignOperation.AddColumn))
                    {
                        // remove all log about this colunt
                        var delLogs = records.Where(r => r.ColumnId == cid).ToList();
                        delLogs.ForEach(r => records.Remove(r));
                        return;
                    }
                }
            }

            // if column created in this log time but not deleted
            if (!records.Any(r => r.ColumnId == cid && r.Operation == DesignOperation.DeleteColumn))
            {
                if (records.Any(r => r.ColumnId == cid && r.Operation == DesignOperation.AddColumn))
                {
                    var delLogs = records.Where(r => r.ColumnId == cid).ToList();
                    delLogs.ForEach(r => records.Remove(r));
                    return;
                }
            }

            // Existed previously column - remove all logs except rename/delete
            var colLogs = records.Where(r => r.ColumnId == cid && r.Operation != DesignOperation.SetColumnName && r.Operation != DesignOperation.DeleteColumn).ToList();
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
                    Operation = MigrationOperation.CreateTable,
                    SchemaName = schema.Name,
                    Table = DesignSchemaConvert.ToStoreDefinition(schema, table)
                };

                tm.OperationCode = Enum.GetName(tm.Operation);
                commands.Add(tm);

            }
            //else
            //{
            //    // table deleted - use original name
            //    var first = records.First(r => r.TableId == id);
            //    var originalName = first.OldValue ?? first.TableName;

            //    var tm = new MigrationCommand
            //    {
            //        Operation = MigrationOperation.DeleteTable,
            //        SchemaName = schema.Name,
            //        TableName = originalName,
            //    };

            //    tm.OperationCode = Enum.GetName(tm.Operation);
            //    commands.Add(tm);
            //}

            // remove all logs about this table - only if table was not deleted
            var tableLogs = records.Where(r => r.TableId == id).ToList();
            tableLogs.ForEach(r => records.Remove(r));
        }

    }
}
