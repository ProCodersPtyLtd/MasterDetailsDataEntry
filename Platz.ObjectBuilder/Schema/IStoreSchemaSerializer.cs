using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder
{
    public interface IStoreSchemaSerializer
    {
        StoreSchema ReadSchema(string json);
        StoreQuery ReadQuery(string json);
        StoreForm ReadForm(string json);
    }
}
