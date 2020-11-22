using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Templates
{
    public class GenerationLogic
    {
        string JsonStorePath = @"StoreData";
        StoreSchema Schema;
        List<StoreQuery> Queries = new List<StoreQuery>();
        TemplateJoin From;
        List<TemplateJoin> Joins;
        JsonStoreSchemaParser Parser = new JsonStoreSchemaParser();

        public void Void()
        {
            Generate();

            foreach (var query in Queries)
            {
                var from = Parser.ReadFrom(query, Schema);
                var joins = Parser.ReadJoins(query, Schema);

            }

            foreach (var query in Queries)
            {
//#>
//    public class <#=query.ReturnTypeName#>
//    {
//<#
                foreach (var field in query.Query.Fields.Values)
                {
                    var table = query.Query.Tables[field.Field.ObjectAlias];
                    var definition = Schema.Definitions[table.TableName];
                    var property = definition.Properties[field.Field.FieldName];
// public <#=property.Type#> 
//#>

//<#
                }
//#>
//}
//<#
            }
//#>
        }

        public

        void Generate()
        {
            var parser = new JsonStoreSchemaParser();
            var path = Path.Combine(GetSolutionDirectory(), JsonStorePath);
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);

                if (Path.GetFileName(file).ToLower() == "schema.json")
                {
                    Schema = parser.ReadSchema(json);
                }
                else
                {
                    var q = parser.ReadQuery(json);
                    Queries.Add(q);
                }
            }

        }


        void GenerateEntities()
        {
            foreach (var query in Queries)
            { 
            }
        }

        // Stubs for tests
        void GenerateFooter()
        {
        }

        void GenerateHeader()
        {
        }

        string GetSolutionDirectory()
        {
            return @"C:\Repos\MasterDetailsDataEntry";
        }
    }
}
