using Platz.ObjectBuilder;
using Platz.ObjectBuilder.Blazor.Model;
using Platz.SqlForms;
using SqlForms.DevSpace.Logic;
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
        IProjectLoader Loader { get; }
        EditWindowDetails ActiveWindow { get; }
        string ActiveWindowName { get; }
        int ActiveWindowIndex { get; }

        void CreateNewProject();
        void CreateNewForm();
        List<StoreSchema> GetProjectSchemas();
        List<StoreQuery> GetProjectQueries();
        List<StoreForm> GetProjectForms();
        List<EditWindowDetails> GetEditWindows();
        bool ActivateWindow(IStoreObject item);
        void ActivateWindow(int index);
        void OpenWindow(IStoreObject item);
        EditWindowType GetStoreObjectType(IStoreObject item);
        void LoadModel(string name);
    }

    public class SpaceController : ISpaceController
    {
        private readonly IProjectLoader _projectLoader;
        private readonly IFormBuilderController _formBuilderController;

        public SpaceProjectDetails Model { get; set; }
        public string ActiveWindowName { get; set; }
        public int ActiveWindowIndex { get; set; }

        public IProjectLoader Loader { get { return _projectLoader; } }

        public EditWindowDetails ActiveWindow
        {
            get
            {
                var wnds = GetEditWindows();
                return wnds.Count > 0 ? wnds[ActiveWindowIndex]: null;
            }
        }

        public SpaceController(IProjectLoader projectLoader, IFormBuilderController formBuilderController)
        {
            _projectLoader = projectLoader;
            _formBuilderController = formBuilderController;
            CreateNewProject();

            // ToDo: remove demo data initialization
            LoadModel(@"C:\Repos\MasterDetailsDataEntry\Seed\App.SqlForms.DevSpace\data\Project1");

            //var sch = "Schema1";
            //var s1 = new StoreSchema { Name = sch };
            //Model.Schemas.Add(new SchemaDetails { Schema = s1 });
            //var f1 = new StoreForm { Name = "CustomerEdit" };
            //ActiveWindowName = "CustomerEdit";
            //Model.Forms.Add(new FormDetails { Form = f1 });
            //Model.Forms.Add(new FormDetails { Form = new StoreForm { Name = "CustomerList" } });
            //Model.Forms.Add(new FormDetails { Form = new StoreForm { Name = "CustomerAddressList" } });
            ////Model.EditWindows.Add(new EditWindowDetails { StoreObject = s1, Type = EditWindowType.Schema });

            //Model.Queries.Add(new QueryDetails { Query = new StoreQuery { Name = "GetCustomerAddressList" } });
            //Model.Queries.Add(new QueryDetails { Query = new StoreQuery { Name = "GetCustomerList" } });

            //Model.EditWindows.Add(new EditWindowDetails { StoreObject = f1, Type = EditWindowType.Form });
            //UpdateFormBuilder();
        }

        private void UpdateFormBuilder()
        {
            _formBuilderController.SetSchemas(GetProjectSchemas());
            _formBuilderController.SetQueries(GetProjectQueries());
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

        public void ActivateWindow(int index)
        {
            var item = Model.EditWindows[index].StoreObject;
            ActivateWindow(item);
            //ActiveWindowIndex = index;
            //ActiveWindowName = Model.EditWindows[index].StoreObject.Name;
        }

        public bool ActivateWindow(IStoreObject item)
        {
            var w = GetEditWindows().FirstOrDefault(x => x.StoreObject == item);

            if (w != null)
            {
                ActiveWindowIndex = GetEditWindows().IndexOf(w);
                ActiveWindowName = w.StoreObject.Name;

                if (GetStoreObjectType(item) == EditWindowType.Form)
                {
                    FormBuilderControllerSwitchModel(item as StoreForm); 
                    //_formBuilderController.SwitchModel(form.Model);
                }

                return true;
            }

            return false;
        }

        private FormDetails FormBuilderControllerSwitchModel(StoreForm item)
        {
            var d = Model.Forms.First(f => f.Form == item);
            var newModel = d.Model == null;

            if (d.Model == null)
            {
                d.Model = new FormBuilderModel(item);
            }

            _formBuilderController.SwitchModel(d.Model);
            UpdateFormBuilder();
            _formBuilderController.RefreshDatasources();

            if (newModel)
            {
                _formBuilderController.SetActive(null);
            }

            return d;
        }

        public void OpenWindow(IStoreObject item)
        {
            if (ActivateWindow(item))
            {
                return;
            }

            var w = new EditWindowDetails { StoreObject = item, Type = GetStoreObjectType(item) };
            Model.EditWindows.Add(w);
            ActivateWindow(item);
        }

        public EditWindowType GetStoreObjectType(IStoreObject item)
        {
            if (item is StoreSchema)
            {
                return EditWindowType.Schema;
            }
            else if (item is StoreQuery)
            {
                return EditWindowType.Query;
            }
            else if (item is StoreForm)
            {
                return EditWindowType.Form;
            }
            else if (item is StoreProject)
            {
                return EditWindowType.ProjectSettings;
            }

            return EditWindowType.Unknown;
        }

        public void LoadModel(string name)
        {
            var project = _projectLoader.Load(name);
            Model = new SpaceProjectDetails();
            Model.Schemas = project.Schemas.Values.Select(x => new SchemaDetails { Schema = x, SchemaMigrations = project.SchemaMigrations[x.Name] }).ToList();
            Model.Queries = project.Queries.Values.Select(x => new QueryDetails { Query = x }).ToList();
            Model.Forms = project.Forms.Values.Select(x => new FormDetails { Form = x }).ToList();
            Model.EditWindows = project.Settings.EditWindows.Select(s => new EditWindowDetails { StoreObject = FindModelStoreObject(s) }).ToList();
            Model.EditWindows.ForEach(w => w.Type = GetStoreObjectType(w.StoreObject));
            UpdateFormBuilder();
        }

        private IStoreObject FindModelStoreObject(string name)
        {
            var result = Model.Schemas.FirstOrDefault(x => x.Schema.Name == name) as IStoreObject ?? 
                Model.Queries.FirstOrDefault(x => x.Query.Name == name) as IStoreObject ??
                Model.Forms.FirstOrDefault(x => x.Form.Name == name) as IStoreObject;

            return result;
        }

        public void CreateNewForm()
        {
            var form = new StoreForm();
            var formDetails = new FormDetails { Form = form };
            ApplyNewFormDefaults(formDetails);
            Model.Forms.Add(formDetails);
            OpenWindow(form);
        }

        private void ApplyNewFormDefaults(FormDetails formDetails)
        {
            formDetails.Form.Name = GetNextFormName();

            if (Model.Schemas.Count == 1)
            {
                formDetails.Form.Schema = Model.Schemas.First().Schema.Name;
                formDetails.Form.Schema = Model.Schemas.First().Schema.Name;
            }
        }

        private string GetNextFormName()
        {
            int i = 0;
            string name;

            do
            {
                i++;
                name = $"Form{i}";
            } while (Model.Forms.Any(f => f.Form.Name == name));

            return name;
        }
    }
}
