using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor
{
    public class DesignSchema
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string DataContextName { get; set; }
        public string Namespace { get; set; }
        public List<DesignTable> Tables { get; set; } = new List<DesignTable>();
        public bool UseIntId { get; set; }
        public bool Changed { get; set; }
        public bool IsNew { get; set; }

        #region Converters
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
        #endregion
    }



    public class DesignTable
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public List<DesignColumn> Columns { get; set; } = new List<DesignColumn>();
        public bool Changed { get; set; }
        public bool IsNew { get; set; }

        public DesignTable()
        {
            Id = Guid.NewGuid();
        }

        public DesignTable(Guid id)
        {
            Id = id;
        }
    }

    public class DesignColumn
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public string TableReference { get; set; }
        public string ColumnReference { get; set; }
        public bool Disabled { get; set; }
        public bool Pk { get; set; }
        public bool Changed { get; set; }
        public bool IsNew { get; set; }

        public DesignColumn()
        {
            Id = Guid.NewGuid();
        }

        public DesignColumn(Guid id)
        {
            Id = id;
        }

        public bool IsEmpty()
        {
            var res = string.IsNullOrWhiteSpace(Name) && !Nullable && string.IsNullOrWhiteSpace(Type) && string.IsNullOrWhiteSpace(Reference);
            return res;
        }

        private string _reference;
        public string Reference { get { return _reference; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Disabled = false;
                }
                else
                {
                    _reference = value;
                    var split = value.Split('.');
                    TableReference = split[0];
                    ColumnReference = split[1];
                    Name = Name ?? value.Replace(".", "");
                    Type = "reference";
                    Disabled = true;
                }
            } 
        }
    }

    public class DesignLogRecord
    {
        public DesignOperation Operation { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string ColumnId { get; set; }
        public string ColumnName { get; set; }
    }

    public enum DesignOperation
    {
        CreateSchema = 1,
        SetSchemaName,
        CreateTable,
        SetTableName,
        DeleteTable,
        SetColumnName,
        DeleteColumn,
        SetColumnType,
        SetColumnNullable,
        SetColumnReference
    }
}
