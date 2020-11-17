using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Platz.ObjectBuilder.Engine
{
    public class EntityFrameworkStoreSchemaReader : IStoreSchemaReader
    {
        public StoreSchema ReadSchema(IStoreSchemaReaderParameters parameters)
        {
            var dr = parameters as EntityFrameworkStoreSchemaReaderParameters;

            if (dr == null)
            {
                throw new Exception($"EntityFrameworkStoreSchemaReader expects EntityFrameworkStoreSchemaReaderParameters.");
            }

            var schema = new StoreSchema { Definitions = new Dictionary<string, StoreDefinition>() };

            using (var db = GetDbContext(dr.DbContextType))
            {
                var entities = db.Model.GetEntityTypes();

                foreach (var entityMetadata in entities)
                {
                    var def = new StoreDefinition { Properties = new Dictionary<string, StoreProperty>() };
                    def.Name = entityMetadata.ClrType.Name;
                    schema.Definitions.Add(def.Name, def);
                    var props = entityMetadata.GetProperties().ToList();

                    for (int i = 0; i < props.Count; i++)
                    {
                        var prop = props[i];

                        var storeProp = new StoreProperty
                        {
                            Name = prop.Name,
                            Order = i,
                            Type = prop.ClrType.Name,
                            Pk = prop.IsPrimaryKey(),
                            Fk = prop.IsForeignKey(),
                            MaxLength = prop.GetMaxLength(),
                            AutoIncrement = prop.ValueGenerated == ValueGenerated.OnAdd
                        };

                        if (prop.IsForeignKey())
                        {
                            var fks = prop.GetContainingForeignKeys();
                            storeProp.ForeignKeys = prop.GetContainingForeignKeys().Select(f => new StoreForeignKey
                            {
                                DefinitionName = f.PrincipalEntityType.ClrType.Name,
                                PropertyName = f.PrincipalKey.Properties.First().Name,
                                CompositePropertyNames = f.PrincipalKey.Properties.Select(p => p.Name).ToList()
                            }).ToList();
                        }

                        def.Properties.Add(storeProp.Name, storeProp);
                    }
                }
            }

            return schema;
        }

        private DbContext GetDbContext(Type dbContextType)
        {
            return Activator.CreateInstance(dbContextType) as DbContext;
        }
    }
}
