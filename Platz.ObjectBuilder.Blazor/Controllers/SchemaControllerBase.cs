using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers
{
    public interface ISchemaMvvm
    {
        string SelectedEditTab { get; }
        bool UseBigIntId { get; }
        DesignTable SelectedTable { get; }

        void SelectSchemaTab();
        void SelectTableTab(int row);
        void SelectTable(DesignTable table);
    }

    public interface ISchemaController : ISchemaMvvm
    {
        DesignSchema Schema { get; }
        SchemaControllerParameters Parameters { get; }

        void SetParameters(SchemaControllerParameters parameters);
        void LoadSchema();
        void NewSchema();

        DesignTable CreateTable();
    }

    public class SchemaControllerParameters
    {
        public string StoreDataPath { get; set; }

        public string Namespace { get; set; }

        public string DataService { get; set; }
    }

    public class SchemaControllerBase : ISchemaController
    {
        protected readonly SchemaTableEditController _tableEditController;

        public SchemaControllerBase(SchemaTableEditController tableEditController)
        {
            _tableEditController = tableEditController;
        }

        public SchemaControllerParameters Parameters { get; protected set; }

        public DesignSchema Schema { get; protected set; } 

        #region Mvvm

        public string SelectedEditTab { get; protected set; } = "Schema";
        public bool UseBigIntId { get; protected set; }
        public DesignTable SelectedTable { get; protected set; }

        public void SelectTable(DesignTable table)
        {
            SelectedTable = table;
        }

        public void SelectSchemaTab()
        {
            SelectedEditTab = "Schema";
        }

        public void SelectTableTab(int row)
        {
            SelectedEditTab = "Table";
        }

        #endregion

        public DesignTable CreateTable()
        {
            var table = _tableEditController.CreateTable(Schema);
            Schema.Tables.Add(table);
            return table;
        }

        public void SetParameters(SchemaControllerParameters parameters)
        {
            Parameters = parameters;
        }

        public void LoadSchema()
        {
        }

        public void NewSchema()
        {
            Schema = new DesignSchema { Name = "New Schema", Version = "0.1", DataContextName = Parameters.DataService };
            // Schema.Tables.Add(new DesignTable { Name = "Table1" });
        }
    }
}
