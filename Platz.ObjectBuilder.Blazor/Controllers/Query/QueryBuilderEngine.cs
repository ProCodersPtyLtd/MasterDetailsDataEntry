using Platz.ObjectBuilder.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Platz.ObjectBuilder.Helpers;
using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Blazor.Validation;
using Platz.SqlForms;

namespace Platz.ObjectBuilder.Blazor.Controllers.Logic
{
    public interface IQueryBuilderEngine
    {
        void SelectPropertiesFromNewTable(IQueryController qc, QueryFromTable newTable);
        StoreQuery GenerateQuery(IQueryModel qm, StoreQueryParameters StoreParameters = null);
        //QueryModel LoadSingleQueryFromStoreQuery(StoreSchema schema, StoreQuery q);
        QueryControllerModel LoadQueryFromStoreQuery(StoreSchema schema, StoreQuery q);
        List<RuleValidationResult> Validate(IQueryControllerModel qm);
        string QueryExprToString(QueryExpression expr, Dictionary<string, string> operatorsMap = null);
    }

    public class QueryBuilderEngine : IQueryBuilderEngine
    {
        private readonly ISqlExpressionEngine _expressions;
        private readonly IBuilderRuleFactory<IQueryBuilderRule, IQueryControllerModel> _ruleEngine;

        public QueryBuilderEngine(ISqlExpressionEngine expressions, IBuilderRuleFactory<IQueryBuilderRule, IQueryControllerModel> ruleEngine)
        {
            _expressions = expressions;
            _ruleEngine = ruleEngine;
        }

        public List<RuleValidationResult> Validate(IQueryControllerModel qm)
        {
            var result = _ruleEngine.ValidateAllRules(qm);
            return result;
        }

        public QueryControllerModel LoadQueryFromStoreQuery(StoreSchema schema, StoreQuery q)
        {
            var result = new QueryControllerModel() { SubQueryList = new List<IQueryModel>(), Schema = schema };
            var loadedSubQueryList = new List<IQueryModel>();

            if (q.Query.SubQueries != null)
            {
                foreach (var qName in q.Query.SubQueries.Keys)
                {
                    var subQuery = ReadQueryTables(loadedSubQueryList, qName, schema, null, q.Query.SubQueries);
                    result.SubQueryList.Add(subQuery);
                }
            }

            result.MainQuery = ReadQueryTables(loadedSubQueryList, "Main", schema, q.Query, q.Query.SubQueries);

            result.StoreParameters = new StoreQueryParameters();
            result.StoreParameters.DataService = q.DataService;
            result.StoreParameters.QueryName = q.Name;
            result.StoreParameters.Namespace = q.Namespace;
            result.StoreParameters.QueryReturnType = q.ReturnTypeName;
            result.Schema.Name = q.SchemaName;
            result.Schema.Version = q.SchemaVersion;

            return result;
        }

        private static IQueryModel ReadQueryTables(List<IQueryModel> loadedSubQueryList, string name,  StoreSchema schema, StoreQueryDefinition queryDef, Dictionary<string, StoreQueryDefinition> subQueries)
        {
            var result = loadedSubQueryList.FirstOrDefault(l => l.Name == name);

            if (result != null)
            {
                return result;
            }

            queryDef = queryDef ?? subQueries[name];
            result = new QueryModel() { Name = name };

            foreach (var t in queryDef.Tables)
            {
                if (schema.Definitions.ContainsKey(t.Value.TableName))
                {
                    // not subquery table 
                    var schemaTable = schema.Definitions[t.Value.TableName];
                    var ft = new QueryFromTable(schemaTable);
                    ft.Alias = t.Value.ObjectAlias;
                    result.FromTables.Add(ft);
                }
                else
                {
                    if (!subQueries.ContainsKey(t.Value.TableName))
                    {
                        throw new Exception($"Table or SubQuery with name '{t.Value.TableName}' not found");
                    }

                    var subQuery = ReadQueryTables(loadedSubQueryList, t.Value.TableName, schema, null, subQueries);
                    var ft = new QueryFromTable(subQuery);
                    ft.Alias = t.Value.ObjectAlias;
                    result.FromTables.Add(ft);
                }
            }

            loadedSubQueryList.Add(result);

            // fields
            foreach (var f in queryDef.Fields.Values)
            {
                var table = result.FromTables.FirstOrDefault(t => t.Alias == f.Field.ObjectAlias);

                if (table == null)
                {
                    throw new Exception($"Table with alias '{f.Field.ObjectAlias}' not found");
                }

                var storeProperty = table.Properties.FirstOrDefault(p => p.Name == f.Field.FieldName);

                if (storeProperty == null)
                {
                    throw new Exception($"Field '{f.Field.FieldName}' not found in table with alias '{f.Field.ObjectAlias}'");
                }

                var newSelectionProperty = new QuerySelectProperty(table, storeProperty.OriginalStoreProperty)
                {
                    IsOutput = f.IsOutput,
                    GroupByFunction = f.GroupByFunction,
                    Having = f.GroupByFilter,
                    Alias = f.FieldAlias != f.Field.FieldName ? f.FieldAlias : ""
                    // ToDo: Filter is not stored and cannot be loaded
                };

                result.SelectionProperties.Add(newSelectionProperty);
            }

            // Joins
            foreach (var j in queryDef.Joins)
            { }

            result.FromTableJoins = queryDef.Joins.Select(j => new TableJoinModel() 
            { 
                IsDeleted = false,
                JoinType = j.JoinType,
                Source = j
            }).ToList();

            // Where
            result.WhereClause = QueryExpressionHelper.QueryExprToString(queryDef.Where.Expression);

            return result;
        }

        //public QueryModel LoadSingleQueryFromStoreQuery(StoreSchema schema, StoreQuery q)
        //{
        //    var result = new QueryModel();
        //    // read parameters
        //    result.StoreParameters.DataService = q.DataService;
        //    result.StoreParameters.QueryName = q.Name;
        //    result.StoreParameters.Namespace = q.Namespace;
        //    result.StoreParameters.QueryReturnType = q.ReturnTypeName;
        //    result.Schema.Name = q.SchemaName;
        //    result.Schema.Version = q.SchemaVersion;

        //    // from
        //    foreach (var t in q.Query.Tables)
        //    {
        //        if (schema.Definitions.ContainsKey(t.Value.TableName))
        //        {
        //            // not subquery table 
        //            var schemaTable = schema.Definitions[t.Value.TableName];
        //            var ft = new QueryFromTable(schemaTable);
        //            ft.Alias = t.Value.ObjectAlias;
        //            result.FromTables.Add(ft);
        //        }
        //        else
        //        {
        //            if (!q.Query.SubQueries.ContainsKey(t.Value.TableName))
        //            {
        //                throw new Exception($"Table or SubQuery with name '{t.Value.TableName}' not found");
        //            }
        //            //var subQuery = q.Query.SubQueries[t.Value.TableName];
        //            //var ft = new QueryFromTable(subQuery);
        //        }
        //    }

        //    // qc.RegenerateTableLinks();

        //    // fields
        //    foreach (var f in q.Query.Fields.Values)
        //    {
        //        var table = result.FromTables.FirstOrDefault(t => t.Alias == f.Field.ObjectAlias);

        //        if (table == null)
        //        {
        //            throw new Exception($"Table with alias '{f.Field.ObjectAlias}' not found");
        //        }

        //        var storeProperty = table.Properties.FirstOrDefault(p => p.Name == f.Field.FieldName);

        //        if (storeProperty == null)
        //        {
        //            throw new Exception($"Field '{f.Field.FieldName}' not found in table with alias '{f.Field.ObjectAlias}'");
        //        }

        //        var newSelectionProperty = new QuerySelectProperty(table, storeProperty.OriginalStoreProperty)
        //        {
        //            IsOutput = f.IsOutput,
        //            GroupByFunction = f.GroupByFunction,
        //            Alias = f.FieldAlias != f.Field.FieldName ? f.FieldAlias : ""
        //            // ToDo: Filter is not stored and cannot be loaded
        //        };

        //        result.SelectionProperties.Add(newSelectionProperty);
        //    }

        //    // Where
        //    result.WhereClause = QueryExpressionHelper.QueryExprToString(q.Query.Where.Expression);

        //    return result;
        //}

        public void SelectPropertiesFromNewTable(IQueryController qc, QueryFromTable newTable)
        {
            var selectedProps = qc.SelectionProperties.ToDictionary(p => p.OutputName, p => p);

            foreach (var p in newTable.Properties)
            {
                if (!selectedProps.ContainsKey(p.Name))
                {
                    p.Selected = true;
                    qc.AddSelectionProperty(newTable, p);
                }
            }
        }

        public StoreQuery GenerateQuery(IQueryModel qc, StoreQueryParameters storeParameters = null)
        {
            try
            {
                var result = new StoreQuery();

                if (storeParameters != null)
                {
                    result.DataService = storeParameters.DataService;
                    result.Name = storeParameters.QueryName;
                    result.Namespace = storeParameters.Namespace;
                    result.ReturnTypeName = storeParameters.QueryReturnType;
                    result.SchemaFile = storeParameters.SchemaFileName;
                }
                else
                {
                    result.Name = qc.Name;
                }

                result.SchemaName = qc.Schema?.Name;
                result.SchemaVersion = qc.Schema?.Version;

                result.Query = new StoreQueryDefinition();

                result.Query.Fields = qc.SelectionProperties.ToDictionary(
                    p => p.OutputName,
                    p => new StoreQueryField
                    {
                        FieldAlias = p.OutputName,
                        IsOutput = p.IsOutput,
                        GroupByFunction = p.GroupByFunction,
                        GroupByFilter = p.Having,
                        Field = new StoreFieldReference { FieldName = p.StoreProperty.Name, ObjectAlias = p.FromTable.Alias }
                    });

                result.Query.Tables = qc.FromTables.ToDictionary(
                    t => t.Alias,
                    t => new StoreTableReference { ObjectAlias = t.Alias, TableName = t.Name, IsSubQuery = t.IsSubQuery }
                    );

                // ToDo: composite foreign keys not supported currently
                List<StoreObjectJoin> joins = qc.FromTableJoins.Where(f => !f.IsDeleted)
                    .Select(f =>
                    {
                        f.Source.JoinType = f.JoinType;
                        return f.Source;
                    }).ToList();

                result.Query.Joins = joins.ToArray();

                var expr = _expressions.BuildExpressionTree(qc.WhereClause);
                var storeExpr = QueryExpressionHelper.ReadFromSqlExpr(expr);
                result.Query.Where = new StoreQueryCondition { Expression = storeExpr };

                var paramsDict = QueryExpressionHelper.GetParamsFromSqlExpr(expr);
                result.Query.Parameters = paramsDict.ToDictionary(d => d.Key, d => new StoreQueryParameter { Name = d.Key, Type = FindStoreProperty(qc, d.Value)?.Type });

                return result;
            }
            catch (Exception exc)
            {
                qc.Errors += "\r\n" + exc.Message;
                return null;
            }
        }

        public string QueryExprToString(QueryExpression expr, Dictionary<string, string> operatorsMap = null)
        {
            return QueryExpressionHelper.QueryExprToString(expr, operatorsMap);
        }

        private StoreProperty FindStoreProperty(IQueryModel qc, string expressionField)
        {
            var split = expressionField.Split('.');

            if (split.Count() < 2)
            {
                return null;
            }

            var alias = split[0];
            var name = split[1];
            var def = qc.FromTables.First(t => t.Alias == alias);
            //var result = def.StoreDefinition.Properties[name];
            var result = def.Properties.First(p => p.Name == name)?.OriginalStoreProperty;
            return result;
        }
    }
}
