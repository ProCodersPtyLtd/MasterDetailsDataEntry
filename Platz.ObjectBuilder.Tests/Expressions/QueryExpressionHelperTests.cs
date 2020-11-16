using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests.Expressions
{
    public class QueryExpressionHelperTests
    {
        [Fact]
        public void ReadFromSqlExprTest()
        {
            var res = new SqlJsonObjectResolver();
            var expEngine = new SqlExpressionEngine(res);
            var expr = expEngine.BuildExpressionTree("c.CustomerId = 12 AND c.CompanyName = 'ahyu' AND c.EmailAddress = @p1 AND c.ModifiedDate = @p2");
            var result = QueryExpressionHelper.ReadFromSqlExpr(expr);
            Assert.Equal("ModifiedDate", result.Right.Left.QueryField.Field.FieldName);
            Assert.Equal("=", result.Right.Operator);
            Assert.Equal("@p2", result.Right.Right.Param);
        }

        [Fact]
        public void GetParamsFromSqlExprTest()
        {
            var res = new SqlJsonObjectResolver();
            var expEngine = new SqlExpressionEngine(res);
            var expr = expEngine.BuildExpressionTree("c.CustomerId = 12 AND c.CompanyName = 'ahyu' AND @p1 =   c.EmailAddress AND (c.ModifiedDate = @p2)");
            var dict = QueryExpressionHelper.GetParamsFromSqlExpr(expr);
            Assert.Equal(2, dict.Values.Count);
            Assert.Contains("@p1", dict.Keys);
            Assert.Contains("@p2", dict.Keys);
            Assert.Equal("c.EmailAddress", dict["@p1"]);
            Assert.Equal("c.ModifiedDate", dict["@p2"]);
        }

    }
}
