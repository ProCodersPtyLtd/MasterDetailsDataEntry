using MasterDetailsDataEntry.Demo.Database;
using Platz.ObjectBuilder.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests
{
    public class EntityFrameworkStoreSchemaReaderTests
    {
        [Fact]
        public void ReadSchemaTest()
        {
            var reader = new EntityFrameworkStoreSchemaReader();
            var ps = new EntityFrameworkStoreSchemaReaderParameters { DbContextType = typeof(AdventureWorksContext) };

            var schema = reader.ReadSchema(ps);

            Assert.Equal(15, schema.Definitions.Count);
            Assert.True(schema.Definitions.ContainsKey("Address"));
        }
    }
}
