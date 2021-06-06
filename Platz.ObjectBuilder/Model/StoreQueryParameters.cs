using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class StoreQueryParameters
    {
        private string _schemaFileName { get; set; }
        public string SchemaFileName 
        { 
            get
            {
                return _schemaFileName ?? Path.Combine(StoreDataPath, $"Schema.json");
            }
        }
        public string StoreDataPath { get; set; }

        public string QueryReturnType { get; set; }

        public string QueryName { get; set; }

        public string Namespace { get; set; }

        public string DataService { get; set; }

        public void SetSchemaFileName(string file)
        {
            _schemaFileName = file;
        }
    }

}
