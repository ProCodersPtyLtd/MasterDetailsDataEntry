using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Engine
{
    public class EntityFrameworkStoreSchemaReader : IStoreSchemaReader
    {
        public StoreSchema ReadSchema(IStoreSchemaReaderParameters parameters)
        {
            var dr = parameters as EntityFrameworkStoreSchemaReaderParameters;

            return null;
        }
    }
}
