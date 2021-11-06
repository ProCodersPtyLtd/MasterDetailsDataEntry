using Platz.ObjectBuilder.Schema;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IQueryController : IQueryControllerModel
    {
        //StoreQueryParameters StoreParameters { get; }
        //StoreSchema Schema { get; }
        string Errors { get; set; }
        string LinqQuery { get; set; }
        string SqlQuery { get; set; }
        //List<IQueryModel> SubQueryList { get; }
        int SelectedQueryIndex { get; set; }
        // used for UI
        IQueryModel SelectedQuery { get; }
        // used only for load/save/validate
        //IQueryModel MainQuery { get; }

        List<TableLink> FromTableLinks { get; }
        List<TableJoinModel> FromTableJoins { get; }
        List<RuleValidationResult> ValidationResults { get; }
        List<QueryFromTable> FromTables { get; }
        List<QuerySelectProperty> SelectionProperties { get; }
        string WhereClause { get; }

        bool NeedRedrawLinks { get; set; }

        void SetSchemas(List<StoreSchema> storeSchemas);
        void Changed();
        void RemoveSubQuery(int index);
        List<DesignQueryObject> GetAvailableQueryObjects();
        void CreateSubQuery(int index);
        void Configure(IQueryControllerConfiguration config);
        void LoadSchema();
        void AddFromTable(DesignQueryObject table);
        void RemoveFromTable(string tableName, string alias);
        QueryFromTable FindFromTable(string tableName, string alias);
        void AddSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void RemoveSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void ApplySelectPropertyFilter(QuerySelectProperty property, string filter);
        string ReviewSelectPropertyFilter(QuerySelectProperty property, string filterText);
        void SetGroupByFunction(QuerySelectProperty property, string filter);
        void SetWhereClause(string text);

        StoreQuery GenerateQuery();
        bool SaveQuery(string path);
        void SaveSchema(string path);
        void LoadFromFile(string path, string fileName);

        string GenerateObjectId(string sfx, int objId, int propId = 0);

        void AliasChanged(string oldAlias, string newAlias);
        //void RegenerateTableLinks();
        void UpdateLinksFromTableJoins();
        void Validate();
        List<string> GetFileList(string path);
        bool FileExists(string path);
        string GenerateFileName(string path);
        void Clear();
        List<string> GetSchemas();
        QueryControllerModel LoadStoreQuery(StoreQuery item);
        void SwitchModel(QueryControllerModel model);
    }
}
