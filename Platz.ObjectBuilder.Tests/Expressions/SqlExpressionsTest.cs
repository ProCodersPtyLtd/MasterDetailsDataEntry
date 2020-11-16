using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Pc.Framework.UnitTests.Expressions
{
    public class SqlExpressionsTest
    {
        [Fact]
        public void ExpressionRunTest()
        {
            var res = new SqlJsonObjectResolver();
            var expEngine = new SqlExpressionEngine(res);
            var expr = expEngine.BuildExpressionTree("((1 = 2 - 1) AND 2 + 3 = 4) OR ( (0 = 0 AND  1 = 1) OR 5 = 7  )");
            var result = expr.GetExprValue();
            Assert.True((bool?)result.Value == true);
        }

        [Fact]
        public void ExpressionParseTest()
        {
            var res = new SqlJsonObjectResolver();
            var expEngine = new SqlExpressionEngine(res);
            var result = expEngine.BuildExpressionTree(
                "((p.project_id = @p1) AND a.allocation_id = @p2) OR ((p.project_id = 0 AND a.allocation_id = 0) OR u.last_name = 'admin')");
            Assert.Equal(SqlExpressionEngine.ExprType.Expr, result.Type);
            Assert.NotNull(result.Left);
            Assert.NotNull(result.Right);
        }

        [Fact]
        public void ExpressionParseAndTest()
        {
            var res = new SqlJsonObjectResolver();
            var expEngine = new SqlExpressionEngine(res);
            var result = expEngine.BuildExpressionTree("c.CustomerId = 12 AND c.CompanyName = 'ahyu' AND c.EmailAddress = '@p1' AND c.ModifiedDate = '@p2'");
            Assert.Equal(SqlExpressionEngine.ExprType.Expr, result.Type);
            Assert.NotNull(result.Left);
            Assert.NotNull(result.Right);
        }
    }
}
