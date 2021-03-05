using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Platz.SqlForms;

namespace Platz.ObjectBuilder.Engine
{
    public class JsonStoreSchemaReader : IStoreSchemaReader
    {
        public StoreSchema ReadSchema(IStoreSchemaReaderParameters parameters)
        {
            throw new NotImplementedException();
        }    
    }
}
