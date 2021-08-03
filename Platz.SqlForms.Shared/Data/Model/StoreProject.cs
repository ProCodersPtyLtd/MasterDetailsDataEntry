using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class StoreProject
    {
        public Dictionary<string, StoreSchema> Schemas { get; set; }
        public Dictionary<string, StoreSchemaMigrations> SchemaMigrations { get; set; }
        public Dictionary<string, StoreQuery> Queries { get; set; }
        public Dictionary<string, StoreForm> Forms { get; set; }
    }
}
