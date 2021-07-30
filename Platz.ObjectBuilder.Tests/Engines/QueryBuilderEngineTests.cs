using Platz.ObjectBuilder.Blazor.Controllers.Logic;
using Platz.ObjectBuilder.Blazor.Validation;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests.Engines
{
    public class QueryBuilderEngineTests
    {
        [Fact]
        public void LoadSimpleSubQueryTest()
        {
            var resolver = new SqlJsonObjectResolver();
            var sqlExpressionEngine = new SqlExpressionEngine(resolver);
            var rules = new BuilderRuleFactory<IQueryBuilderRule, IQueryControllerModel>();
            var engine = new QueryBuilderEngine(sqlExpressionEngine, rules);
            var storage = new FileStoreSchemaStorage();
            var parameters = new StorageParameters { FileName = "Getq1List.json", Path = @"C:\Repos\MasterDetailsDataEntry\SqlForms.Demo\StoreNew\" };
            var storeQuery = storage.LoadQuery(parameters);
            var reader = new JsonStoreSchemaReader(storage);
            var schema = reader.ReadSchema(new JsonStoreSchemaReaderParameters { SchemaFile = @"C:\Repos\MasterDetailsDataEntry\SqlForms.Demo\StoreNew\Crm2.Schema.json" });
            var result = engine.LoadQueryFromStoreQuery(schema, storeQuery);
        }
    }
}
