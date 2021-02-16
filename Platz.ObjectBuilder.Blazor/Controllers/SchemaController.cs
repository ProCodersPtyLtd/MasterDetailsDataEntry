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
        int ListSelectedRow { get; set; }

        void SetParameters(SchemaControllerParameters parameters);
        void LoadSchema();
        void NewSchema();

        DesignTable CreateTable();
        void DeleteTable(DesignTable table);
        void UpdateLog(DesignOperation operation, DesignTable table = null, DesignColumn column = null);
    }

    public class SchemaControllerParameters
    {
        public string StoreDataPath { get; set; }

        public string Namespace { get; set; }

        public string DataService { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SchemaController : ISchemaController
    {
        private readonly SchemaTableDesignController _tableController;

        private List<DesignLogRecord> _designRecords = new List<DesignLogRecord>();

        public SchemaControllerParameters Parameters { get; protected set; }
        public DesignSchema Schema { get; protected set; } 
        public int ListSelectedRow { get; set; }

        public SchemaController(SchemaTableDesignController tableController)
        {
            _tableController = tableController;
        }

        /// <summary>
        /// Keeps log that allows to record migrations and undo/redo
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        public void UpdateLog(DesignOperation operation, DesignTable table = null, DesignColumn column = null)
        {
        }


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
            ListSelectedRow = 0;
        }

        public void SelectTableTab(int row)
        {
            SelectedEditTab = "Table";
        }

        #endregion

        public DesignTable CreateTable()
        {
            var table = _tableController.CreateTable(Schema);
            Schema.Tables.Add(table);
            UpdateLog(DesignOperation.CreateTable, table);
            return table;
        }

        public void DeleteTable(DesignTable table)
        {
            Schema.Tables.Remove(table);
            SelectedTable = null;
            SelectSchemaTab();
            UpdateLog(DesignOperation.DeleteTable, table);
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
            Schema = new DesignSchema { Name = "New Schema", Version = "1.0", DataContextName = Parameters.DataService, Changed = true, IsNew = true };
            UpdateLog(DesignOperation.CreateSchema);
        }
    }
}
