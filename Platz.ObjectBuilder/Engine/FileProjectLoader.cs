﻿using Platz.ObjectBuilder;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Engine
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
            result.Name = Path.GetFileName(name).Replace(".project.json", "");
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
            SaveFile(location, project.Name, project.Settings, StoreProjectItemType.Project);

            // forms
            foreach (var form in project.Forms.Values)
            {
                SaveFile(location, form.Name, form, StoreProjectItemType.Form);
            }

            ClearRenamed(project, location, renames);
        }

        private void SaveFile(string location, string name, object data, StoreProjectItemType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("File cannot have empty name");
            }

            var file = name + "." + GetProjectItemExtension(type);
            file = Path.Combine(location, file);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(file, json);
        }

        private void ClearRenamed(StoreProject project, string location, List<ObjectRenameItem> renames)
        {
            foreach (var rn in renames)
            {
                var name = rn.OrignalName + "." + GetProjectItemExtension(rn.Type);
                var fileName = Path.Combine(location, name);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        private string GetProjectItemExtension(StoreProjectItemType type)
        {
            switch (type)
            {
                case StoreProjectItemType.Project:
                    return "project.json";
                case StoreProjectItemType.Form:
                    return "form.json";
                case StoreProjectItemType.Query:
                    return "query.json";
                case StoreProjectItemType.Schema:
                    return "schema.json";
            }

            return "";
        }

        public StoreSchema LoadSchema(string folderName, string name)
        {
            throw new NotImplementedException();
        }

        public StoreQuery LoadQuery(string folderName, string name)
        {
            throw new NotImplementedException();
        }
    }
}
