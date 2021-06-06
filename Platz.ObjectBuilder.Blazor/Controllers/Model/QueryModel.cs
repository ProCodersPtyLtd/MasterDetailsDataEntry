using Platz.ObjectBuilder.Blazor.Validation;
using Platz.ObjectBuilder.Schema;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class QueryModel : IQueryModel
    {
        public string Name { get; set; }
        public StoreQueryParameters StoreParameters { get; set; } = new StoreQueryParameters();
        public StoreSchema Schema { get; set; } = new StoreSchema();
        public List<QueryFromTable> FromTables { get; set; } = new List<QueryFromTable>();
        public List<QuerySelectProperty> SelectionProperties { get; set; } = new List<QuerySelectProperty>();
        public List<TableLink> FromTableLinks { get; set; } = new List<TableLink>();
        public List<TableJoinModel> FromTableJoins { get; set; } = new List<TableJoinModel>();
        public List<RuleValidationResult> ValidationResults { get; set; } = new List<RuleValidationResult>();
        public string WhereClause { get; set; } = "";
        public string Errors { get; set; } = "";
    }

    
}
