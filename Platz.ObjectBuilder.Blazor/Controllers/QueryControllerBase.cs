using Microsoft.EntityFrameworkCore;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using Platz.ObjectBuilder.Helpers;
using Platz.ObjectBuilder.Schema;
using System;
using System.Collections.Generic;
using System.IO;
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
        StoreQueryParameters StoreParameters { get; }

        void SetParameters(IQueryControllerParameters parameters);
        void LoadSchema();
        void AddFromTable(StoreDefinition table);
        void RemoveFromTable(string tableName, string alias);
        void AddSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void RemoveSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void ApplySelectPropertyFilter(QuerySelectProperty property, string filter);
        void SetWhereClause(string text);

        StoreQuery GenerateQuery();
        void SaveQuery(string path);
    }

    public interface IQueryControllerParameters
    { }

    public class StoreQueryParameters
    {
        public string StoreDataPath { get; set; }

        public string QueryReturnType { get; set; }

        public string QueryName { get; set; }

        public string Namespace { get; set; }

        public string DataService { get; set; }
    }

    public abstract class QueryControllerBase : IQueryController
    {
        public StoreQueryParameters StoreParameters { get; set; } = new StoreQueryParameters();
        public StoreSchema Schema { get; private set; }
        public List<QueryFromTable> FromTables { get; private set; } = new List<QueryFromTable>();
        public List<QuerySelectProperty> SelectionProperties { get; private set; } = new List<QuerySelectProperty>();
        public string WhereClause { get; private set; } = "";
        public string Errors { get; set; } = "";

        protected IStoreSchemaReader _reader;
        protected IStoreSchemaStorage _storage;
        protected IStoreSchemaReaderParameters _readerParameters;
        protected IObjectResolver _resolver;
        protected SqlExpressionEngine _expressions;

        public QueryControllerBase()
        {
        }

        public abstract void SetParameters(IQueryControllerParameters parameters);

        public void SaveQuery(string path)
        {
            var fileName = Path.Combine(path, $"{StoreParameters.QueryName}.json");
            var parameters = new StorageParameters { FileName = fileName };
            var query = GenerateQuery();
            _storage.SaveQuery(query, parameters);
        }

        public void LoadSchema()
        {
            Schema = _reader.ReadSchema(_readerParameters);
        }

        public StoreQuery GenerateQuery()
        {
            try
            {
                var result = new StoreQuery() 
                { 
                    DataService = StoreParameters.DataService,
                    Name = StoreParameters.QueryName,
                    Namespace = StoreParameters.Namespace,
                    ReturnTypeName = StoreParameters.QueryReturnType,
                    SchemaName = Schema.Name,
                    SchemaVersion = Schema.Version
                };

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

                // ToDo: composite foreign keys not supported currently
                var joins = new List<StoreObjectJoin>();
                var tables = FromTables.ToList();
                
                var foreignKeys = tables.SelectMany(t => t.Properties, (t, p) => new { Tbl = t, Prop = p }).Where(p => 
                    p.Prop.StoreProperty.Fk && tables.Any(d => d.StoreDefinition.Name == p.Prop.StoreProperty.ForeignKeys.First().DefinitionName)).ToList();

                foreach (var reference in foreignKeys)
                {
                    var leftTable = tables.First(t => t.StoreDefinition.Name == reference.Prop.StoreProperty.ForeignKeys.First().DefinitionName);
                    var rightTable = tables.First(t => t.StoreDefinition.Name == reference.Tbl.StoreDefinition.Name);

                    var join = new StoreObjectJoin
                    {
                        JoinType = "inner",
                        LeftObjectAlias = leftTable.Alias,
                        LeftField = reference.Prop.StoreProperty.ForeignKeys.First().PropertyName,
                        RightObjectAlias = rightTable.Alias,
                        RightField = reference.Prop.StoreProperty.Name
                    };

                    joins.Add(join);
                }

                result.Query.Joins = joins.ToArray();

                var expr = _expressions.BuildExpressionTree(WhereClause);
                var storeExpr = QueryExpressionHelper.ReadFromSqlExpr(expr);
                result.Query.Where = new StoreQueryCondition { Expression = storeExpr };

                return result;
            }
            catch(Exception exc)
            {
                Errors += "\r\n" + exc.Message;
                return null;
            }
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
