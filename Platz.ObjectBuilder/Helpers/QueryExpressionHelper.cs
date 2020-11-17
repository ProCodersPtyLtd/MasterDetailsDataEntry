using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Helpers
{
    public static class QueryExpressionHelper
    {
        // <param, operand>
        public static Dictionary<string, string> GetParamsFromSqlExpr(SqlExpressionEngine.Expr expr, Dictionary<string, string> dict = null)
        { 
            if (dict == null)
            {
                dict = new Dictionary<string, string>();
            }

            if (expr == null)
            {
                return dict;
            }

            if (expr.Type == SqlExpressionEngine.ExprType.Expr && expr.Left != null && expr.Right != null)
            {
                if (expr.Left.Type == SqlExpressionEngine.ExprType.Param)
                {
                    dict[expr.Left.Value] = expr.Right.Value;
                }

                if (expr.Right.Type == SqlExpressionEngine.ExprType.Param)
                {
                    dict[expr.Right.Value] = expr.Left.Value;
                }
            }

            GetParamsFromSqlExpr(expr.Left, dict);
            GetParamsFromSqlExpr(expr.Right, dict);

            return dict;
        }

        public static QueryExpression ReadFromSqlExpr(SqlExpressionEngine.Expr expr)
        {
            if (expr == null || expr.Type == SqlExpressionEngine.ExprType.None)
            {
                return null;
            }

            var storeExpr = new QueryExpression();

            switch (expr.Type)
            {
                case SqlExpressionEngine.ExprType.Field:
                    var fieldSplit = expr.Value.Split('.');
                    storeExpr.QueryField = new StoreQueryField { Field = new StoreFieldReference { ObjectAlias = fieldSplit[0], FieldName = fieldSplit[1] } };
                    break;
                case SqlExpressionEngine.ExprType.Param:
                    storeExpr.Param = expr.Value;
                    break;
                case SqlExpressionEngine.ExprType.Number:
                case SqlExpressionEngine.ExprType.StringLiteral:
                    storeExpr.Value = expr.Value;
                    break;
                case SqlExpressionEngine.ExprType.Expr:
                    break;
            }

            storeExpr.Operator = expr.Operator;
            storeExpr.Left = ReadFromSqlExpr(expr.Left);
            storeExpr.Right = ReadFromSqlExpr(expr.Right);

            return storeExpr;
        }

        //public static string ToString(this QueryExpression storeExpr)
        //{
        //    return ExprToString(storeExpr);
        //}

        public static string QueryExprToString(QueryExpression expr)
        {
            if (expr == null)
            {
                return "";
            }

            if (expr.QueryField != null)
            {
                return $"{expr.QueryField.Field.ObjectAlias}.{expr.QueryField.Field.FieldName}";
            }
            else if (expr.Param != null)
            {
                return expr.Param;
            }
            else if (expr.Value != null)
            {
                return expr.Value.ToString();
            }

            var left = expr.Left.Operator == null ? $"{QueryExprToString(expr.Left)}" : $"({QueryExprToString(expr.Left)})";
            var right = expr.Right.Operator == null ? $"{QueryExprToString(expr.Right)}" : $"({QueryExprToString(expr.Right)})";

            var result = $"{left} {expr.Operator} {right}";
            return result;
        }
    }
}
