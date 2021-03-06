﻿using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Helpers;
using Platz.ObjectBuilder.Interfaces;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Engine
{
    public class EntityFrameworkQueryGenerator : IQueryGenerator
    {
        public string GenerateQuery(string schemaFileName, string queryFileName)
        {
            var sb = new StringBuilder();
            var schemaJson = File.ReadAllText(schemaFileName);
            var queryJson = File.ReadAllText(queryFileName);
            var parser = new JsonStoreSchemaParser();

            var schema = parser.ReadSchema(schemaJson);
            var query = parser.ReadQuery(queryJson);

            var parameters = parser.ReadParameters(query);

            var sq = SortSubQueries(query);


            if (query.Query.Parameters != null && query.Query.Parameters.Any())
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];

                    sb.AppendLine(@$"
    var {p.Name} = ({p.Type})parameters[{i}];");
                }
            }

            foreach (var subQuery in sq)
            {
                AppendSingleQuery(sb, parser, schema, subQuery.Value, subQuery.Key, "", query.Query.SubQueries);
                sb.AppendLine();
            }

            AppendSingleQuery(sb, parser, schema, query.Query, "query", query.ReturnTypeName, query.Query.SubQueries);
            sb.AppendLine();
            AppendQueryReturnType(sb, parser, schema, query);

            return sb.ToString();
        }

        private void AppendQueryReturnType(StringBuilder sb, JsonStoreSchemaParser parser, StoreSchema schema, StoreQuery query)
        {
            var subQueries = query.Query.SubQueries;

            sb.Append(@$"
    public  class {query.ReturnTypeName}
    {{");

            foreach (var field in query.Query.Fields.Values.Where(f => f.IsOutput))
            {
                var table = query.Query.Tables[field.Field.ObjectAlias];

                if (table.IsSubQuery)
                {
                    string originalPropertyName = field.FieldAlias;

                    while (table.IsSubQuery)
                    {
                        var sq = subQueries[table.TableName];
                        var sqf = sq.Fields[originalPropertyName];
                        originalPropertyName = sqf.Field.FieldName;
                        var sqt = sq.Tables[sqf.Field.ObjectAlias];
                        table = sqt;
                    }

                    var definition = schema.Definitions[table.TableName];
                    var property = definition.Properties[originalPropertyName];

                    sb.Append(@$"
        public {property.Type} {field.FieldAlias} {{ get; set; }}");
                }
                else
                {
                    var definition = schema.Definitions[table.TableName];
                    var property = definition.Properties[field.Field.FieldName];

                    sb.Append(@$"
        public {property.Type} {field.FieldAlias} {{ get; set; }}");

                }
            }

            sb.Append(@$"
    }}");

        }

        private List<KeyValuePair<string, StoreQueryDefinition>> SortSubQueries(StoreQuery query)
        {
            if (query.Query.SubQueries == null)
            {
                return new List<KeyValuePair<string, StoreQueryDefinition>>();
            }

            var result = query.Query.SubQueries.ToList();
            result.Sort(delegate (KeyValuePair<string, StoreQueryDefinition> a, KeyValuePair<string, StoreQueryDefinition> b)
            {
                if (a.Value.Tables.Any(t => t.Value.TableName == b.Key))
                {
                    return 1;
                }

                if (b.Value.Tables.Any(t => t.Value.TableName == a.Key))
                {
                    return -1;
                }

                return 0;
            });

            return result;
        }

        private static void AppendSingleGroupByQuery(StringBuilder sb, JsonStoreSchemaParser parser, StoreSchema schema, StoreQueryDefinition query, string queryName,
            string returnTypeName, Dictionary<string, StoreQueryDefinition> subQueries)
        {
            var cfgParams = new QueryControllerParameters();
            var cfg = new EntityFrameworkQueryControllerConfiguration(cfgParams);
            ISqlExpressionEngine expressionEngine = new SqlExpressionEngine(cfg.Resolver);

            var from = parser.ReadFrom(query, schema);
            var joins = parser.ReadJoins(query, schema);
            var where = parser.QueryExprToString(query.Where.Expression, JsonStoreSchemaParser.CSharpOperatorsMap);

            string dbPrefix;
            dbPrefix = query.Tables[from.LeftObjectAlias].IsSubQuery ? "" : "db.";

            sb.Append(@$"
    var {queryName} =
        from {from.LeftObjectAlias} in {dbPrefix}{from.LeftObject}");

            foreach (var j in joins)
            {
                dbPrefix = query.Tables[j.RightObjectAlias].IsSubQuery ? "" : "db.";

                sb.Append(@$"
        join {j.RightObjectAlias} in {dbPrefix}{j.RightObject} on {j.LeftObjectAlias}.{j.LeftField} equals {j.RightObjectAlias}.{j.RightField}");

            }

            // where
            if (!string.IsNullOrWhiteSpace(where))
            {
                sb.Append(@$"
        where {where}");

            }

            // group by
            //var gbAliases = joins.Select(j => j.RightObjectAlias)
            var gbAliases = query.Fields.Values.Where(f => f.GroupByFunction == "Group By").Select(f => f.Field.ObjectAlias).Distinct();
            var gbFields = query.Fields.Values.Where(f => f.GroupByFunction == "Group By").Select(f => $"{f.Field.ObjectAlias}.{f.FieldAlias}").Distinct();

            string groupVar = "group1";

            sb.Append(@$"
        group new {{ {string.Join(',', gbAliases)} }} by new {{ {string.Join(',', gbFields)} }} into {groupVar}");

            // having
            var havingFields = query.Fields.Values.Where(f => !string.IsNullOrWhiteSpace(f.GroupByFilter));

            if (havingFields.Any())
            {
                var having = "";

                foreach (var field in havingFields)
                {
                    if (!string.IsNullOrWhiteSpace(having))
                    {
                        having += " && ";
                    }

                    var grpbExpr = GetGroupBySelectField(sb, field, groupVar, gbAliases);
                    
                    // convert filter fo EF LINQ expression
                    var filterExpr = GetFilterQueryExpression(expressionEngine, field, field.GroupByFilter);
                    var cond = parser.QueryExprToString(filterExpr, JsonStoreSchemaParser.CSharpOperatorsMap);
                    cond = cond.Replace($"{field.Field.ObjectAlias}.{field.FieldAlias} ", "");
                    having += $"{grpbExpr} {cond}";
                }

                sb.Append(@$"
        where {having}");

            }

            sb.Append(@$"
        select new {returnTypeName}
        {{");

            foreach (var field in query.Fields.Values.Where(f => f.IsOutput))
            {
                var exprField = GetGroupBySelectField(sb, field, groupVar, gbAliases);
                
                sb.Append(@$"
            {field.FieldAlias} = {exprField},");

            }

            sb.Append(@$"
        }};");
        }

        private static QueryExpression GetFilterQueryExpression(ISqlExpressionEngine expressions, StoreQueryField field, string filter)
        {
            var filterClause = $"{field.Field.ObjectAlias}.{field.FieldAlias} {filter}";
            var expr = expressions.BuildExpressionTree(filterClause);
            var result = QueryExpressionHelper.ReadFromSqlExpr(expr);
            return result;
        }

        private static string GetGroupBySelectField(StringBuilder sb, StoreQueryField field, string groupVar, IEnumerable<string> gbAliases)
        {
            string function;

            if (string.IsNullOrWhiteSpace(field.GroupByFunction))
            {
                function = $"{field.Field.ObjectAlias}.{field.Field.FieldName}";
            }
            else if (field.GroupByFunction == "Group By")
            {
                function = $"{groupVar}.Key.{field.Field.FieldName}";
            }
            else if (field.GroupByFunction == "Count")
            {
                function = $"{groupVar}.Count()";
            }
            else
            {
                function = $"{groupVar}.{field.GroupByFunction}(x => x.{field.Field.ObjectAlias}.{field.Field.FieldName})";
            }

            return function;
        }

        private static void AppendSingleQuery(StringBuilder sb, JsonStoreSchemaParser parser, StoreSchema schema, StoreQueryDefinition query, string queryName,
            string returnTypeName, Dictionary<string, StoreQueryDefinition> subQueries)
        {
            if (query.Fields.Values.Any(f => f.GroupByFunction == "Group By"))
            {
                AppendSingleGroupByQuery(sb, parser, schema, query, queryName,returnTypeName, subQueries);
                return;
            }

            var from = parser.ReadFrom(query, schema);
            var joins = parser.ReadJoins(query, schema);
            var where = parser.QueryExprToString(query.Where.Expression, JsonStoreSchemaParser.CSharpOperatorsMap);

            string dbPrefix;
            dbPrefix = query.Tables[from.LeftObjectAlias].IsSubQuery ? "" : "db.";

            sb.Append(@$"
    var {queryName} =
        from {from.LeftObjectAlias} in {dbPrefix}{from.LeftObject}");

            foreach (var j in joins)
            {
                dbPrefix = query.Tables[j.RightObjectAlias].IsSubQuery ? "" : "db.";

                sb.Append(@$"
        join {j.RightObjectAlias} in {dbPrefix}{j.RightObject} on {j.LeftObjectAlias}.{j.LeftField} equals {j.RightObjectAlias}.{j.RightField}");

            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                sb.Append(@$"
        where {where}");

            }

            sb.Append(@$"
        select new {returnTypeName}
        {{");

            foreach (var field in query.Fields.Values.Where(f => f.IsOutput))
            {
                var table = query.Tables[field.Field.ObjectAlias];

                if (table.IsSubQuery)
                {
                    var definition = subQueries[table.TableName];
                    var property = definition.Fields[field.Field.FieldName];

                    sb.Append(@$"
            {field.FieldAlias} = {field.Field.ObjectAlias}.{field.Field.FieldName},");

                }
                else
                {
                    var definition = schema.Definitions[table.TableName];
                    var property = definition.Properties[field.Field.FieldName];

                    sb.Append(@$"
            {field.FieldAlias} = {field.Field.ObjectAlias}.{field.Field.FieldName},");

                }
            }

            sb.Append(@$"
        }};");

        }
    }
}
