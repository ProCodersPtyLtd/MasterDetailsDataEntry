using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor
{
    public class DesignSchema
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string DataContextName { get; set; }
        public string Namespace { get; set; }
        public List<DesignTable> Tables { get; set; } = new List<DesignTable>();
        public bool UseIntId { get; set; }
        public bool Changed { get; set; }
        public bool IsNew { get; set; }
    }



    public class DesignTable
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public List<DesignColumn> Columns { get; set; } = new List<DesignColumn>();
        public bool Changed { get; set; }
        public bool IsNew { get; set; }

        public DesignTable()
        {
            Id = Guid.NewGuid();
        }

        public DesignTable(Guid id)
        {
            Id = id;
        }
    }

    public class DesignColumn
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public string TableReference { get; set; }
        public string ColumnReference { get; set; }
        public bool Disabled { get; set; }
        public bool Pk { get; set; }
        public bool Changed { get; set; }
        public bool IsNew { get; set; }

        public DesignColumn()
        {
            Id = Guid.NewGuid();
        }

        public DesignColumn(Guid id)
        {
            Id = id;
        }

        public bool IsEmpty()
        {
            var res = string.IsNullOrWhiteSpace(Name) && !Nullable && string.IsNullOrWhiteSpace(Type) && string.IsNullOrWhiteSpace(Reference);
            return res;
        }

        private string _reference;
        public string Reference { get { return _reference; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Disabled = false;
                }
                else
                {
                    _reference = value;
                    var split = value.Split('.');
                    TableReference = split[0];
                    ColumnReference = split[1];
                    Name = Name ?? value.Replace(".", "");
                    Type = "reference";
                    Disabled = true;
                }
            } 
        }
    }

    public class DesignLogRecord
    {
        public DesignOperation Operation { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
    }

    public enum DesignOperation
    {
        CreateSchema = 1,
        SetSchemaName,
        CreateTable,
        SetTableName,
        DeleteTable,
        SetColumnName,
        DeleteColumn,
        SetColumnType,
        SetColumnNullable,
        SetColumnReference
    }
}
