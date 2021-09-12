using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class StoreProject : IStoreObject
    {
        public string Name { get; set; }
        public StoreProjectSettings Settings { get; set; } = new StoreProjectSettings();
        public bool Validated { get; set; }

        [JsonIgnore]
        public Dictionary<string, StoreSchema> Schemas { get; set; }
        [JsonIgnore]
        public Dictionary<string, StoreSchemaMigrations> SchemaMigrations { get; set; }
        [JsonIgnore]
        public Dictionary<string, StoreQuery> Queries { get; set; }
        [JsonIgnore]
        public Dictionary<string, StoreForm> Forms { get; set; }
    }

    public class StoreProjectSettings
    {
        public List<string> EditWindows { get; set; } = new List<string>();
    }

    public enum StoreProjectItemType
    {
        Project,
        Schema,
        SchemaMigrations,
        Query,
        Form
    }
}
