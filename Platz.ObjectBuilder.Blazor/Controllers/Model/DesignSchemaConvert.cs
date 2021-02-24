using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor
{
    public static class DesignSchemaConvert
    {
        public static DesignSchema FromStoreSchema(StoreSchema s)
        {
            var ds = new DesignSchema
            {
                Name = s.Name,
                DataContextName = s.DbContextName,
                Namespace = s.Namespace,
                Version = s.Version,
            };

            if (s.Settings.ContainsKey("UseIntId"))
            {
                ds.UseIntId = bool.Parse(s.Settings["UseIntId"]);
            }

            foreach (var d in s.Definitions.Values)
            {
                var t = new DesignTable()
                {
                    Name = d.Name,
                };

                ds.Tables.Add(t);

                t.Columns = d.Properties.Select(x => x.Value).OrderBy(x => x.Order).Select(c => new DesignColumn()
                {
                    Name = c.Name,
                    Pk = c.Pk,
                    Type = c.Type,
                    Nullable = c.Nullable,
                    Reference = GetDesignForeignKey(c),
                }).ToList();
            }

            return ds;
        }

        private static string GetDesignForeignKey(StoreProperty c)
        {
            if (c.ForeignKeys.Any())
            {
                var fk = c.ForeignKeys.First();
                return $"{fk.DefinitionName}.{fk.PropertyName}";
            }

            return null;
        }

        public static StoreDefinition ToStoreDefinition(DesignTable t)
        {
            var d = new StoreDefinition { Name = t.Name, Properties = new Dictionary<string, StoreProperty>() };
            int i = 0;

            d.Properties = t.Columns.Where(c => !c.IsEmpty()).ToDictionary(c => c.Name, c => new StoreProperty
            {
                Name = c.Name,
                Type = c.Type,
                Pk = c.Pk,
                Fk = !string.IsNullOrWhiteSpace(c.Reference),
                Nullable = c.Nullable,
                AutoIncrement = c.Pk,
                ForeignKeys = GetForeignKeysForColumn(t, c),
                Order = i++
            });

            return d;
        }

        public static StoreSchema ToStoreSchema(DesignSchema s)
        {
            var schema = new StoreSchema
            {
                Name = s.Name,
                Version = s.Version,
                DbContextName = s.DataContextName,
                Namespace = s.Namespace,
                Definitions = new Dictionary<string, StoreDefinition>(),
                Settings = new Dictionary<string, string>(),
            };

            var tags = new List<string>();
            schema.Settings.Add("UseIntId", s.UseIntId.ToString());

            //schema.Definitions = Schema.Tables.ToDictionary(t => t.Name, t => new StoreDefinition 
            //{ 
            //    Name = t.Name,
            //    Properties = t.Columns.ToDictionary(c => c.Name, c => new StoreProperty 
            //    {
            //        Name = c.Name,
            //        Type = c.Type,
            //        Pk = c.Pk,
            //        Fk = !string.IsNullOrWhiteSpace(c.Reference),
            //        Nullable = c.Nullable,
            //        AutoIncrement = c.Pk,

            //    })
            //});

            foreach (var t in s.Tables)
            {
                int i = 0;
                schema.Definitions[t.Name] = new StoreDefinition { Name = t.Name, Properties = new Dictionary<string, StoreProperty>() };

                schema.Definitions[t.Name].Properties = t.Columns.Where(c => !c.IsEmpty()).ToDictionary(c => c.Name, c => new StoreProperty
                {
                    Name = c.Name,
                    Type = c.Type,
                    Pk = c.Pk,
                    Fk = !string.IsNullOrWhiteSpace(c.Reference),
                    Nullable = c.Nullable,
                    AutoIncrement = c.Pk,
                    ForeignKeys = GetForeignKeysForColumn(t, c),
                    Order = i++
                });
            }

            schema.Tags = tags.ToArray();
            return schema;
        }

        private static List<StoreForeignKey> GetForeignKeysForColumn(DesignTable t, DesignColumn c)
        {
            if (!string.IsNullOrWhiteSpace(c.TableReference) && !string.IsNullOrWhiteSpace(c.ColumnReference))
            {
                return (new StoreForeignKey[] { new StoreForeignKey { DefinitionName = c.TableReference, PropertyName = c.ColumnReference } }).ToList();
            }

            return new List<StoreForeignKey>();
        }
    }
}
