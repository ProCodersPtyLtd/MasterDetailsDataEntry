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
        string ActiveWindow { get; }

        void CreateNewProject();
        List<StoreSchema> GetProjectSchemas();
        List<StoreQuery> GetProjectQueries();
        List<StoreForm> GetProjectForms();
        List<EditWindowDetails> GetEditWindows();
    }

    public class SpaceController : ISpaceController
    {
        public SpaceProjectDetails Model { get; set; }
        public string ActiveWindow { get; set; }

        public SpaceController()
        {
            CreateNewProject();

            // ToDo: remove demo data initialization
            ActiveWindow = "Schema1";
            var s1 = new StoreSchema { Name = ActiveWindow };
            Model.Schemas.Add(new SchemaDetails { Schema = s1 });
            var f1 = new StoreForm { Name = "CustomerEdit" };
            Model.Forms.Add(new FormDetails { Form = f1 });
            Model.EditWindows.Add(new EditWindowDetails { StoreObject = f1, Type = EditWindowType.Form });
            Model.EditWindows.Add(new EditWindowDetails { StoreObject = s1, Type = EditWindowType.Schema });
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

        public List<EditWindowDetails> GetEditWindows()
        {
            var result = Model.EditWindows;
            return result;
        }
    }
}
