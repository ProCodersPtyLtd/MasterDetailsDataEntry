using Platz.ObjectBuilder.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests.Engines
{
    public class EntityFrameworkQueryGeneratorTests
    {
        [Fact]
        public void GenerateSimpleJoinQueryTest()
        {
            var gen = new EntityFrameworkQueryGenerator();
            var s = "schema.json";
            var q = "GetPersonAddressList.json";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetFullPath(Path.Combine(path, @"..\..\..\.."));
            path = Path.Combine(path, @"SqlForms.Demo\StoreNew\");
            s = Path.Combine(path, s);
            q = Path.Combine(path, q);
            var query = gen.GenerateQuery(s, q);
        }
 
        [Fact]
        public void GenerateSubQueryJoinQueryTest()
        {
            var gen = new EntityFrameworkQueryGenerator();
            var s = "schema.json";
            var q = "Getq0List.json";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetFullPath(Path.Combine(path, @"..\..\..\.."));
            path = Path.Combine(path, @"SqlForms.Demo\StoreNew\");
            s = Path.Combine(path, s);
            q = Path.Combine(path, q);
            var query = gen.GenerateQuery(s, q);
        }

        [Fact]
        public void GenerateEfSubQueryJoinQueryTest()
        {
            var gen = new EntityFrameworkQueryGenerator();
            var s = "Schema.json";
            var q = "GetCustAddrSubQueryList.json";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetFullPath(Path.Combine(path, @"..\..\..\.."));
            path = Path.Combine(path, @"SqlForms.Demo\StoreData\");
            s = Path.Combine(path, s);
            q = Path.Combine(path, q);
            var query = gen.GenerateQuery(s, q);
        }

        [Fact]
        public void GenerateSimpleJoinGroupByQueryTest()
        {
            var gen = new EntityFrameworkQueryGenerator();
            var s = "Schema.json";
            var q = "GetWeightByCostList.json";
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.GetFullPath(Path.Combine(path, @"..\..\..\.."));
            path = Path.Combine(path, @"SqlForms.Demo\StoreData\");
            s = Path.Combine(path, s);
            q = Path.Combine(path, q);
            var query = gen.GenerateQuery(s, q);
        }
    }
}
