using Microsoft.EntityFrameworkCore;
using Platz.ObjectBuilder.Engine;
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

        void SetParameters(IQueryControllerParameters parameters);
        void LoadSchema();
        void AddFromTable(StoreDefinition table);
        void RemoveFromTable(string tableName, string alias);
        void AddSelectionProperty(QueryFromTable table, QueryFromProperty property);
        void RemoveSelectionProperty(QueryFromTable table, QueryFromProperty property);
    }

    public interface IQueryControllerParameters
    { }

    public abstract class QueryControllerBase : IQueryController
    {
        public StoreSchema Schema { get; private set; }
        public List<QueryFromTable> FromTables { get; private set; } = new List<QueryFromTable>();
        public List<QuerySelectProperty> SelectionProperties { get; private set; } = new List<QuerySelectProperty>();

        protected IStoreSchemaReader _reader;
        protected IStoreSchemaReaderParameters _readerParameters;

        public QueryControllerBase()
        {
        }

        public abstract void SetParameters(IQueryControllerParameters parameters);

        public void LoadSchema()
        {
            Schema = _reader.ReadSchema(_readerParameters);
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
    }

    
}
