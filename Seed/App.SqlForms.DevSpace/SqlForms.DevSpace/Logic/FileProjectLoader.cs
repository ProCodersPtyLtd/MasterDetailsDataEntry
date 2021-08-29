using Platz.ObjectBuilder;
using Platz.SqlForms;
using SqlForms.DevSpace.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Logic
{
    public class FileProjectLoader : IProjectLoader
    {
        public List<string> GetFolders(string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            var result = Directory.GetDirectories(location).ToList();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">full path to folder where project file *.project.json located</param>
        /// <returns></returns>
        public StoreProject Load(string folderName)
        {
            var result = new StoreProject();
            result.Schemas = new Dictionary<string, StoreSchema>();
            result.SchemaMigrations = new Dictionary<string, StoreSchemaMigrations>();
            result.Queries = new Dictionary<string, StoreQuery>();
            result.Forms = new Dictionary<string, StoreForm>();

            var dir = folderName;
            
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var name = Directory.GetFiles(dir, "*.project.json").FirstOrDefault();

            if (name == null)
            {
                return result;
            }

            var projectJson = File.ReadAllText(name);
            result.Name = name;
            result.Settings = JsonSerializer.Deserialize<StoreProjectSettings>(projectJson);

            var files = Directory.GetFiles(dir, "*.schema.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var schema = JsonSerializer.Deserialize<StoreSchema>(json);
                result.Schemas.Add(schema.Name, schema);

                var migrationFile = file.Substring(0, file.Length - ".schema.json".Length) + ".schema.migrations.json";
                json = File.ReadAllText(migrationFile);
                var migration = JsonSerializer.Deserialize<StoreSchemaMigrations>(json);
                result.SchemaMigrations.Add(schema.Name, migration);
            }

            files = Directory.GetFiles(dir, "*.query.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var query = JsonSerializer.Deserialize<StoreQuery>(json);
                result.Queries.Add(query.Name, query);
            }

            files = Directory.GetFiles(dir, "*.form.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var form = JsonSerializer.Deserialize<StoreForm>(json);
                result.Forms.Add(form.Name, form);
            }

            return result;
        }

        public void SaveAll(StoreProject project, string location, List<ObjectRenameItem> renames)
        {
        }
    }
}
