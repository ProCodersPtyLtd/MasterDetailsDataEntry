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
    public interface IQueryModel
    {
        string Name { get; set; }
        StoreSchema Schema { get; set; }
        List<QueryFromTable> FromTables { get; set; }
        List<QuerySelectProperty> SelectionProperties { get; set; }
        string WhereClause { get; set; }
        string Errors { get; set; }
        StoreQueryParameters StoreParameters { get; }
        List<TableLink> FromTableLinks { get; set; }
        List<TableJoinModel> FromTableJoins { get; set; }
        List<RuleValidationResult> ValidationResults { get; set; }
    }
}
