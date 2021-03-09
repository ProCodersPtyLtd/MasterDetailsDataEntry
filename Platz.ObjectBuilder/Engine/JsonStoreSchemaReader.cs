using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Platz.SqlForms;
using System.IO;

namespace Platz.ObjectBuilder.Engine
{
    public class JsonStoreSchemaReader : IStoreSchemaReader
    {
        private readonly IStoreSchemaStorage _schemaStorage;

        public JsonStoreSchemaReader(IStoreSchemaStorage schemaStorage)
        {
            _schemaStorage = schemaStorage;
        }

        public StoreSchema ReadSchema(IStoreSchemaReaderParameters parameters)
        {
            var fn = (parameters as JsonStoreSchemaReaderParameters).SchemaFile;
            var path = Path.GetDirectoryName(fn);
            var fileName = Path.GetFileName(fn);
            var schema = _schemaStorage.LoadSchema(new StorageParameters { FileName = fileName, Path = path });
            return schema;
        }    
    }
}
