using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Platz.SqlForms
{
    // trying to define classes that serialized to jsonSchema
    public class StoreSchema
    {
        public string Name { get; set; }
        public string DbContextName { get; set; }
        public string Namespace { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public string[] Tags { get; set; }
        public string Version { get; set; }
        public Guid VersionKey { get; set; }

        public Dictionary<string, StoreDefinition> Definitions { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }

    // The first property in these classes is a Key in Dictionary
    // Sql Table
    public class StoreDefinition
    {
        [Key]
        public string Name { get; set; }

        public string Type { get; set; }
        public string Comment { get; set; }
        public int Order { get; set; }

        public Dictionary<string, StoreProperty> Properties { get; set; } = new Dictionary<string, StoreProperty>();
    }

    // Sql Field
    public class StoreProperty
    {
        [Key]
        public virtual string Name { get; set; }

        // types: string, number
        public virtual string Type { get; set; }

        public virtual bool Nullable { get; set; }

        public virtual string Comment { get; set; }

        public virtual int MinLength { get; set; }
        public virtual int? MaxLength { get; set; }
        public virtual bool Pk { get; set; }
        public virtual bool AutoIncrement { get; set;  }

        // FK
        public virtual bool Fk { get; set; }
        

        public virtual bool ExternalId { get; set; }
        public virtual int Order { get; set; }

        public virtual List<StoreForeignKey> ForeignKeys { get; set; }
    }

    public class StoreForeignKey
    {
        public string DefinitionName { get; set; }
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public List<string> CompositePropertyNames { get; set; }
    }
}
