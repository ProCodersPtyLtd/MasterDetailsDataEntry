using Platz.ObjectBuilder;
using Platz.ObjectBuilder.Blazor;
using Platz.ObjectBuilder.Blazor.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Model
{
    public class SpaceProjectDetails
    {
        public List<EditWindowDetails> EditWindows { get; set; } = new List<EditWindowDetails>();
        public List<SchemaDetails> Schemas { get; set; } = new List<SchemaDetails>();
        public List<QueryDetails> Queries { get; set; } = new List<QueryDetails>();
        public List<FormDetails> Forms { get; set; } = new List<FormDetails>();
    }

    public class SchemaDetails
    {
        public StoreSchema Schema { get; set; }
        public StoreSchemaMigrations SchemaMigrations { get; set; }
        public DesignSchema Model { get; set; }
        public DesignSchemaMigrations ModelSchemaMigrations { get; set; }
    }

    public class QueryDetails
    {
        public StoreQuery Query { get; set; }
        public QueryControllerModel Model { get; set; }
    }
    public class FormDetails
    {
        public StoreForm Form { get; set; }
        public FormBuilderModel Model { get; set; }
    }
}
