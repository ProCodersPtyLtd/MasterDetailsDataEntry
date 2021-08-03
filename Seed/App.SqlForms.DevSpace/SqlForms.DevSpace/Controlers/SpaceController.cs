using Platz.SqlForms;
using SqlForms.DevSpace.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Controlers
{
    public interface ISpaceController
    {
        SpaceProjectDetails Model { get; }
        void CreateNewProject();
        List<StoreSchema> GetProjectSchemas();
        List<StoreQuery> GetProjectQueries();
        List<StoreForm> GetProjectForms();
    }

    public class SpaceController : ISpaceController
    {
        public SpaceProjectDetails Model { get; set; }

        public SpaceController()
        {
            CreateNewProject();

            // ToDo: remove demo data initialization
            Model.Schemas.Add(new SchemaDetails { Schema = new Platz.SqlForms.StoreSchema { Name = "Schema1" } });
        }

        public void CreateNewProject()
        {
            Model = new SpaceProjectDetails();
        }

        public List<StoreSchema> GetProjectSchemas()
        {
            var result = Model.Schemas.Select(s => s.Schema).OrderBy(s => s.Name).ToList();
            return result;
        }

        public List<StoreQuery> GetProjectQueries()
        {
            var result = Model.Queries.Select(s => s.Query).OrderBy(s => s.Name).ToList();
            return result;
        }

        public List<StoreForm> GetProjectForms()
        {
            var result = Model.Forms.Select(s => s.Form).OrderBy(s => s.Name).ToList();
            return result;
        }
    }
}
