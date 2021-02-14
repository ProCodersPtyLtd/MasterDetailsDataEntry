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
        public List<DesignTable> Tables { get; set; } = new List<DesignTable>();
        public bool UseBigIntId { get; set; }
    }

    public class DesignTable
    {
        public string Name { get; set; }
        public List<DesignColumn> Columns { get; set; } = new List<DesignColumn>();
        public bool Changed { get; set; }
    }

    public class DesignColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public string TableReference { get; set; }
        public string ColumnReference { get; set; }
        public bool Disabled { get; set; }
        public bool Pk { get; set; }

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
}
