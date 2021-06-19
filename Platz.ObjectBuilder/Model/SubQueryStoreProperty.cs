using Platz.ObjectBuilder.Schema;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class SubQueryStoreProperty : StoreProperty
    {
        // public string QueryName { get; set; }
        public StoreProperty Source { get; private set; }
        public QuerySelectProperty QuerySelectProperty { get; private set; }

        public override string Name { get { return QuerySelectProperty.OutputName; } }
        public override string Type { get { return Source.Type; } }
        public override bool Nullable { get { return Source.Nullable; } }
        public override string Comment { get { return Source.Comment; } }
        public override int MinLength { get { return Source.MinLength; } }
        public override int? MaxLength { get { return Source.MaxLength; } }
        public override bool Pk { get { return false; } }
        public override bool AutoIncrement  { get { return Source.AutoIncrement; } }
        public override bool Fk { get { return Source.Fk; } }
        public override bool ExternalId { get { return Source.ExternalId; } }
        public override int Order { get { return Source.Order; } }
        public override List<StoreForeignKey> ForeignKeys { get { return Source.ForeignKeys; } }

        public SubQueryStoreProperty(QuerySelectProperty queryProperty)
        {
            QuerySelectProperty = queryProperty;
            Source = queryProperty.StoreProperty;
        }
    }
}
