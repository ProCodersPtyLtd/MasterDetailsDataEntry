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

    public interface IStoreObjectDetails
    {
        string DisplayName { get; }
        void DiscardChanges();
    }

    public class SchemaDetails : IStoreObjectDetails
    {
        public StoreSchema Schema { get; set; }
        public StoreSchemaMigrations SchemaMigrations { get; set; }
        public DesignSchema Model { get; set; }
        public DesignSchemaMigrations ModelSchemaMigrations { get; set; }

        public string DisplayName => throw new NotImplementedException();

        public void DiscardChanges()
        {
            throw new NotImplementedException();
        }
    }

    public class QueryDetails : IStoreObjectDetails
    {
        public StoreQuery Query { get; set; }
        public QueryControllerModel Model { get; set; }

        public string DisplayName
        {
            get
            {
                return Model?.DisplayName ?? Query.Name;
            }
        }

        public void DiscardChanges()
        {
            throw new NotImplementedException();
        }
    }
    public class FormDetails : IStoreObjectDetails
    {
        public StoreForm Form { get; set; }
        public FormBuilderModel Model { get; set; }

        public string DisplayName 
        {
            get 
            {
                return Model?.DisplayName ?? Form.Name;
            }
        }

        public void DiscardChanges()
        {
            Model = null;
        }
    }
}
