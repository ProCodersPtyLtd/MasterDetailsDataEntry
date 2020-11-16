using Microsoft.EntityFrameworkCore;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Helpers;
using Platz.ObjectBuilder.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor
{
    public interface IQueryController
    {
        StoreSchema Schema { get; }
        List<QueryFromTable> FromTables { get; }
        List<QuerySelectProperty> SelectionProperties { get; }
        string WhereClause { get; }
        string Errors { get; set; }

        void SetParameters(IQueryControllerParameters parameters);
        void LoadSchema();
        void AddFromTable(StoreDefinition table);
        void RemoveFromTable(string tableName, string alias);
        void AddSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void RemoveSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void ApplySelectPropertyFilter(QuerySelectProperty property, string filter);
        void SetWhereClause(string text);

        StoreQuery GenerateQuery();
    }

    public interface IQueryControllerParameters
    { }

    public abstract class QueryControllerBase : IQueryController
    {
        public StoreSchema Schema { get; private set; }
        public List<QueryFromTable> FromTables { get; private set; } = new List<QueryFromTable>();
        public List<QuerySelectProperty> SelectionProperties { get; private set; } = new List<QuerySelectProperty>();
        public string WhereClause { get; private set; } = "";
        public string Errors { get; set; } = "";

        protected IStoreSchemaReader _reader;
        protected IStoreSchemaReaderParameters _readerParameters;
        protected IObjectResolver _resolver;
        protected SqlExpressionEngine _expressions;

        public QueryControllerBase()
        {
        }

        public abstract void SetParameters(IQueryControllerParameters parameters);

        public void LoadSchema()
        {
            Schema = _reader.ReadSchema(_readerParameters);
        }

        public StoreQuery GenerateQuery()
        {
            var result = new StoreQuery();
            result.Query = new StoreQueryDefinition();
            
            result.Query.Fields = SelectionProperties.ToDictionary(
                p => p.Alias ?? p.StoreProperty.Name,
                p => new StoreQueryField 
                {
                    FieldAlias = p.Alias ?? p.StoreProperty.Name,
                    Field = new StoreFieldReference { FieldName = p.StoreProperty.Name, ObjectAlias = p.FromTable.Alias }
                });

            result.Query.Tables = FromTables.ToDictionary(
                t => t.Alias,
                t => new StoreTableReference { ObjectAlias = t.Alias, TableName = t.StoreDefinition.Name }
                );

            // result.Query.Joins = FromTables.ToDictionary(
            
            var expr = _expressions.BuildExpressionTree(WhereClause);
            var storeExpr = QueryExpressionHelper.ReadFromSqlExpr(expr);
            result.Query.Where = new StoreQueryCondition { Expression = storeExpr };

            return result;
        }

        

        public void AddFromTable(StoreDefinition table)
        {
            var ft = new QueryFromTable(table);
            ft.Alias = GetDefaultAlias(ft);
            FromTables.Add(ft);
        }

        public void RemoveFromTable(string tableName, string alias)
        {
            var table = FromTables.Single(t => t.Alias == alias && t.StoreDefinition.Name == tableName);
            FromTables.Remove(table);
        }

        private string GetDefaultAlias(QueryFromTable ft)
        {
            var count = FromTables.Where(t => t.StoreDefinition.Name == ft.StoreDefinition.Name).Count();
            var sfx = count > 0 ? (count + 1).ToString() : "";
            var used = FromTables.Select(t => t.Alias).ToList();

            for (int i = 1; i <= 5; i++)
            {
                var alias = ft.StoreDefinition.Name.Substring(0, i).ToLower() + sfx;

                if (!used.Contains(alias))
                {
                    return alias;
                }
            }

            // alias not found
            return "";
        }

        public void AddSelectionProperty(QueryFromTable table, QueryFromProperty property)
        {
            var newSelectProperty = new QuerySelectProperty(table, property.StoreProperty) { IsOutput = true };
            SelectionProperties.Add(newSelectProperty);
        }

        public void RemoveSelectionProperty(QueryFromTable table, QueryFromProperty property)
        {
            var item = SelectionProperties.FirstOrDefault(s => s.StoreProperty.Name == property.StoreProperty.Name && s.FromTable.Alias == table.Alias);

            if (item != null)
            {
                SelectionProperties.Remove(item);
            }
        }

        public void ApplySelectPropertyFilter(QuerySelectProperty property, string filterText)
        {
            var filter = filterText?.Trim();

            if (string.IsNullOrWhiteSpace(filter))
            {
                // find and remove previous condition
            }
            else
            {
                // add new condition
                var filterOperator = "=";

                var operators = _resolver.GetCompareOperators();
                
                foreach (var o in operators)
                {
                    if (filter.StartsWith(o))
                    {
                        filterOperator = o;
                        filter = filter.Substring(o.Count()).Trim();
                        break;
                    }
                };

                var filterValue = filter;
                char quote = _resolver.GetStringLiteralSymbol();

                if (filterValue.First() != '@' && (property.StoreProperty.Type == "String" || property.StoreProperty.Type == "DateTime"))
                {
                    if (filterValue.First() != quote)
                    {
                        filterValue = quote + filterValue;
                    }

                    if (filterValue.Last() != quote)
                    {
                        filterValue = filterValue + quote;
                    }
                }

                if ( !string.IsNullOrWhiteSpace(WhereClause))
                {
                    WhereClause += " AND ";
                }

                WhereClause += $"{property.FromTable.Alias}.{property.StoreProperty.Name} {filterOperator} {filterValue}";
            }
        }

        public void SetWhereClause(string text)
        {
            WhereClause = text;
            CheckWhereClause();
        }

        private void CheckWhereClause()
        {
            try
            {
                var result = _expressions.BuildExpressionTree(WhereClause);
            }
            catch(Exception exc)
            {
                Errors += "\r\n" + exc.Message;
            }
        }
    }

    
}
