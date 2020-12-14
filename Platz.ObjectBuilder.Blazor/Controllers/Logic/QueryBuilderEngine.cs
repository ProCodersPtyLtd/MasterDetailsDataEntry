using Platz.ObjectBuilder.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Platz.ObjectBuilder.Helpers;
using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Blazor.Controllers.Validation;

namespace Platz.ObjectBuilder.Blazor.Controllers.Logic
{
    public interface IQueryBuilderEngine
    {
        void SelectPropertiesFromNewTable(IQueryController qc, QueryFromTable newTable);
        StoreQuery GenerateQuery(IQueryModel qm);
        List<RuleValidationResult> Validate(IQueryModel qm);
    }

    public class QueryBuilderEngine : IQueryBuilderEngine
    {
        private readonly ISqlExpressionEngine _expressions;
        private readonly IObjectBuilderRuleFactory _ruleEngine;

        public QueryBuilderEngine(ISqlExpressionEngine expressions, IObjectBuilderRuleFactory ruleEngine)
        {
            _expressions = expressions;
            _ruleEngine = ruleEngine;
        }

        public List<RuleValidationResult> Validate(IQueryModel qm)
        {
            var result = _ruleEngine.ValidateAllRules(qm);
            return result;
        }

        public void SelectPropertiesFromNewTable(IQueryController qc, QueryFromTable newTable)
        {
            var selectedProps = qc.SelectionProperties.ToDictionary(p => p.OutputName, p => p);

            foreach (var p in newTable.Properties)
            {
                if (!selectedProps.ContainsKey(p.StoreProperty.Name))
                {
                    p.Selected = true;
                    qc.AddSelectionProperty(newTable, p);
                }
            }
        }

        public StoreQuery GenerateQuery(IQueryModel qc)
        {
            try
            {
                var result = new StoreQuery()
                {
                    DataService = qc.StoreParameters.DataService,
                    Name = qc.StoreParameters.QueryName,
                    Namespace = qc.StoreParameters.Namespace,
                    ReturnTypeName = qc.StoreParameters.QueryReturnType,
                    SchemaName = qc.Schema.Name,
                    SchemaVersion = qc.Schema.Version
                };

                result.Query = new StoreQueryDefinition();

                result.Query.Fields = qc.SelectionProperties.ToDictionary(
                    p => p.OutputName,
                    p => new StoreQueryField
                    {
                        FieldAlias = p.OutputName,
                        IsOutput = p.IsOutput,
                        Field = new StoreFieldReference { FieldName = p.StoreProperty.Name, ObjectAlias = p.FromTable.Alias }
                    });

                result.Query.Tables = qc.FromTables.ToDictionary(
                    t => t.Alias,
                    t => new StoreTableReference { ObjectAlias = t.Alias, TableName = t.StoreDefinition.Name }
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
            var result = def.StoreDefinition.Properties[name];
            return result;
        }
    }
}
