using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor
{
    public class SchemaTableDesignController
    {
        public DesignTable Table { get; protected set; }
        public string TableScript { get; set; }

        private DesignSchema _schema;
        
        public List<DesignColumn> Columns { get { return Table.Columns; } }

        public string[] DataTypes = new string[] { "string", "int", "date", "money", "guid", "reference" };

        public void Load(DesignSchema schema, DesignTable table)
        {
            _schema = schema;
            Table = table;
        }

        public void SetScript(string script)
        {
            // ToDo: Parse script and update columns
        }

        public List<string> GetTablePrimaryKeys()
        {
            var result = _schema.Tables.OrderBy(t => t.Name).Select(t => $"{t.Name}.{t.Columns.First().Name}").ToList();
            return result;
        }

        public DesignTable CreateTable(DesignSchema schema, string name = null)
        {
            _schema = schema;
            Table = new DesignTable { Name = name ?? "New Table", Changed = true };
            SetTableDefaults(Table);
            return Table;
        }

        public void Update()
        {
            CheckNewColumnDefault(Table);
            CheckReferenceColumnDefaults(Table);
            TableScript = GenerateScript(Table);
        }

        private string GenerateScript(DesignTable table)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"TABLE {table.Name} (");

            foreach (var c in table.Columns)
            {
                if (c.IsEmpty())
                {
                    continue;
                }
                    
                var nullable = c.Nullable ? "?" : "";
                var reference = string.IsNullOrWhiteSpace(c.Reference) ? "" : $" {c.Reference}";
                sb.AppendLine($"    {c.Name} {c.Type}{nullable}{reference},");
            }

            sb.AppendLine(")");

            return sb.ToString();
        }

        private void CheckReferenceColumnDefaults(DesignTable table)
        {
        }

        public List<string> GetDataTypes()
        {
            return DataTypes.ToList();
        }

        private void SetTableDefaults(DesignTable table)
        {
            // create Id column
            if (table.Columns.Count == 0)
            {
                table.Columns.Add(new DesignColumn { Disabled = true });
            }

            // Set Id column
            var PkColumn = table.Columns[0];
            PkColumn.Name = "Id";
            PkColumn.Type = _schema.UseIntId ? "int" : "guid";
            PkColumn.Pk = true;

            CheckNewColumnDefault(table);
        }

        private void CheckNewColumnDefault(DesignTable table)
        {
            var last = table.Columns.Last();

            //if (!string.IsNullOrWhiteSpace(last.Name) || last.Nullable || !string.IsNullOrWhiteSpace(last.Type) || !string.IsNullOrWhiteSpace(last.Reference))
            if (!last.IsEmpty())
            {
                table.Columns.Add(new DesignColumn {});
            }
        }
    }

    
}
