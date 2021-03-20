using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Platz.ObjectBuilder.Engine
{
    public class FileStoreSchemaStorage : IStoreSchemaStorage
    {
        public bool FileExists(StorageParameters parameters)
        {
            var fileName = Path.Combine(parameters.Path, parameters.FileName);
            var result = File.Exists(fileName);
            return result;
        }

        public List<string> GetFileNames(StorageParameters parameters)
        {
            var files = Directory.GetFiles(parameters.Path);
            var result = files.Select(f => Path.GetFileName(f)).ToList();
            return result;
        }

        public StoreQuery LoadQuery(StorageParameters parameters)
        {
            var fileName = Path.Combine(parameters.Path, parameters.FileName);
            var json = File.ReadAllText(fileName);
            var result = JsonSerializer.Deserialize<StoreQuery>(json);
            return result;
        }

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

        public StoreSchema LoadSchema(StorageParameters parameters)
        {
            var fileName = Path.Combine(parameters.Path, parameters.FileName);
            var json = File.ReadAllText(fileName);
            var result = JsonSerializer.Deserialize<StoreSchema>(json);
            return result;
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
        public void SaveMigration(StoreSchemaMigrations package, StorageParameters parameters)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var json = System.Text.Json.JsonSerializer.Serialize(package, options);

            using (var sw = new StreamWriter(parameters.FileName, false))
            {
                sw.Write(json);
            }
        }

        public StoreSchemaMigrations LoadMigrations(StorageParameters parameters)
        {
            using var sw = new StreamReader(parameters.FileName);
            var json = sw.ReadToEnd();
            var package = System.Text.Json.JsonSerializer.Deserialize< StoreSchemaMigrations>(json);
            return package;
        }
    }
}
