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

    public class TableJoinModel
    {
        public string JoinType { get; set; }
        public bool IsDeleted { get; set; }
        public StoreObjectJoin Source { get; set; }
    }

    public class TableLink
    {
        public int Order { get; set; }
        public string PrimaryRefId { get; set; }
        public string ForeignRefId { get; set; }
        public StoreObjectJoin Source { get; set; }
    }

    public class BoundingClientRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class LinePoints
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
}
