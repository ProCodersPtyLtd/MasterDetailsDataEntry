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
