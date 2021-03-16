using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Platz.SqlForms;

namespace Platz.ObjectBuilder
{
    public class JsonStoreSchemaParser : IStoreSchemaSerializer
    {
        public StoreSchema ReadSchema(string json)
        {
            var data = JsonSerializer.Deserialize<StoreSchema>(json);

            foreach(var tk in data.Definitions.Keys)
            {
                var table = data.Definitions[tk];
                table.Name = tk;

                foreach(var pk in table.Properties.Keys)
                {
                    table.Properties[pk].Name = pk;
                }
            }

            return data;
        }

        public StoreQuery ReadQuery(string json)
        {
            var data = JsonSerializer.Deserialize<StoreQuery>(json);
            var query = data.Query;

            // populate key props in Dictionaries 

            if (query.Fields != null)
            {
                foreach (var fk in query.Fields.Keys)
                {
                    var f = query.Fields[fk];
                    f.FieldAlias = fk;
                }
            }

            if (query.Tables != null)
            {
                foreach (var tk in query.Tables.Keys)
                {
                    var table = query.Tables[tk];
                    table.ObjectAlias = tk;
                }
            }

            if (query.SubQueries != null)
            {
                foreach (var sk in query.SubQueries.Keys)
                {
                    var q = query.SubQueries[sk];
                    q.QueryAlias = sk;
                }
            }

            return data;
        }

        public List<StoreQueryParameter> ReadParameters(StoreQuery query)
        {
            if (query.Query.Parameters == null)
            {
                return new List<StoreQueryParameter>();
            }

            var result = query.Query.Parameters.Values.Select(p => new StoreQueryParameter { Name = p.Name.Replace("@", ""), Type = p.Type }).ToList();
            return result;
        }

        public TemplateJoin ReadFrom(StoreQuery query, StoreSchema schema)
        {
            TemplateJoin result;
            var join = query.Query.Joins.FirstOrDefault();

            if (join != null)
            {
                var table = query.Query.Tables[join.LeftObjectAlias].TableName;
                result = new TemplateJoin { LeftField = join.LeftField, LeftObjectAlias = join.LeftObjectAlias, LeftObject = table };
            }
            else
            {
                var table = query.Query.Tables.Values.First();
                result = new TemplateJoin { LeftObjectAlias = table.ObjectAlias, LeftObject = table.TableName };
            }

            return result;
        }

        public List<TemplateJoin> ReadJoins(StoreQuery query, StoreSchema schema)
        {
            var result = new List<TemplateJoin>();
            var rightTables = new HashSet<string>();

            foreach (var join in query.Query.Joins)
            {
                var leftTable = query.Query.Tables[join.LeftObjectAlias].TableName;
                var rightTable = query.Query.Tables[join.RightObjectAlias].TableName;
                TemplateJoin tj;

                if (rightTables.Contains(join.RightObjectAlias))
                {
                    // flip
                    tj = new TemplateJoin
                    {
                        LeftField = join.RightField,
                        LeftObjectAlias = join.RightObjectAlias,
                        LeftObject = rightTable,

                        RightField = join.LeftField,
                        RightObjectAlias = join.LeftObjectAlias,
                        RightObject = leftTable
                    };
                }
                else
                {
                    tj = new TemplateJoin
                    {
                        LeftField = join.LeftField,
                        LeftObjectAlias = join.LeftObjectAlias,
                        LeftObject = leftTable,
                        RightField = join.RightField,
                        RightObjectAlias = join.RightObjectAlias,
                        RightObject = rightTable
                    };

                    rightTables.Add(join.RightObjectAlias);
                }

                result.Add(tj);
            }

            return result;
        }

        public string QueryExprToString(QueryExpression expr, Dictionary<string, string> operatorsMap = null)
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
                return expr.Param.Replace("@", "");
            }
            else if (expr.Value != null)
            {
                return expr.Value.ToString();
            }

            var left = expr.Left.Operator == null ? $"{QueryExprToString(expr.Left, operatorsMap)}" : $"({QueryExprToString(expr.Left, operatorsMap)})";
            var right = expr.Right.Operator == null ? $"{QueryExprToString(expr.Right, operatorsMap)}" : $"({QueryExprToString(expr.Right, operatorsMap)})";

            var mappedOperator = expr.Operator;

            if (operatorsMap != null && operatorsMap.ContainsKey(mappedOperator.ToLower()))
            {
                mappedOperator = operatorsMap[mappedOperator.ToLower()];
            }

            var result = $"{left} {mappedOperator} {right}";
            return result;
        }

        public static Dictionary<string, string> CSharpOperatorsMap = new Dictionary<string, string>()
        {
            { "or", "||" },
            { "and", "&&" },
            { "=", "==" },
            { "<>", "!=" },
        };

        public static Dictionary<string, string> SqlOperatorsMap = new Dictionary<string, string>()
        {
            { "or", "OR" },
            { "and", "AND" },
            { "=", "=" },
            { "<>", "<>" },
        };

        public string QueryExprToSql(QueryExpression expr, Dictionary<string, string> operatorsMap = null)
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

            var left = expr.Left.Operator == null ? $"{QueryExprToSql(expr.Left, operatorsMap)}" : $"({QueryExprToSql(expr.Left, operatorsMap)})";
            var right = expr.Right.Operator == null ? $"{QueryExprToSql(expr.Right, operatorsMap)}" : $"({QueryExprToSql(expr.Right, operatorsMap)})";

            var mappedOperator = expr.Operator;

            if (operatorsMap != null && operatorsMap.ContainsKey(mappedOperator.ToLower()))
            {
                mappedOperator = operatorsMap[mappedOperator.ToLower()];
            }

            var result = $"{left} {mappedOperator} {right}";
            return result;
        }

        public string DesignShemaTypeToCSharp(StoreProperty p)
        {
            if (p.Pk && p.Type == "int")
            {
                //return "long";
                return "int";
            }

            string result;

            if (p.Fk)
            {
                result = DesignSchemaTypesMap[p.ForeignKeys.First().Type];
            }
            else
            {
                if (DesignSchemaTypesMap.ContainsKey(p.Type))
                {
                    result = DesignSchemaTypesMap[p.Type];
                }
                else
                {
                    result = p.Type;
                }
            }

            if (result != "string" && p.Nullable)
            {
                result = result + "?";
            }

            return result;
        }

        //public string DesignShemaTypeToCSharp(StoreQueryField p)
        //{
        //    if (p.Pk && p.Type == "int")
        //    {
        //        //return "long";
        //        return "int";
        //    }

        //    string result;

        //    if (p.Fk)
        //    {
        //        result = DesignSchemaTypesMap[p.ForeignKeys.First().Type];
        //    }
        //    else
        //    {
        //        if (DesignSchemaTypesMap.ContainsKey(p.Type))
        //        {
        //            result = DesignSchemaTypesMap[p.Type];
        //        }
        //        else
        //        {
        //            result = p.Type;
        //        }
        //    }

        //    if (result != "string" && p.Nullable)
        //    {
        //        result = result + "?";
        //    }

        //    return result;
        //}

        public string ToSqlType(string type)
        {
            return DesignSchemaTypesToSqlMap[type];
        }

        public static Dictionary<string, string> DesignSchemaTypesMap = new Dictionary<string, string>()
        {
            { "guid", "Guid" },
            { "string", "string" },
            { "int", "int" },
            { "date", "DateTime" },
            { "decimal", "decimal" },
            { "money", "decimal" },
            { "bool", "bool" },
        };

        public static Dictionary<string, string> DesignSchemaTypesToSqlMap = new Dictionary<string, string>()
        {
            { "guid", "UNIQUEIDENTIFIER" },
            { "string", "VARCHAR(MAX)" },
            { "int", "INT" },
            { "date", "DATE2" },
            { "decimal", "MONEY" },
            { "money", "MONEY" },
            { "bool", "BIT" },
        };
    }
}
