using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Platz.ObjectBuilder.Engine
{
    public class FileStoreSchemaStorage : IStoreSchemaStorage
    {
        public void SaveQuery(StoreQuery query, StorageParameters parameters)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var json = System.Text.Json.JsonSerializer.Serialize(query, options);

            using (var sw = new StreamWriter(parameters.FileName, false))
            {
                sw.Write(json);
            }
        }

        public void SaveSchema(StoreSchema schema, StorageParameters parameters)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var json = System.Text.Json.JsonSerializer.Serialize(schema, options);

            using (var sw = new StreamWriter(parameters.FileName, false))
            {
                sw.Write(json);
            }
        }
    }
}
