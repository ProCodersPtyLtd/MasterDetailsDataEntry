using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder
{
    public interface IStoreSchemaReader
    {
        StoreSchema ReadSchema(IStoreSchemaReaderParameters parameters);
    }
}
