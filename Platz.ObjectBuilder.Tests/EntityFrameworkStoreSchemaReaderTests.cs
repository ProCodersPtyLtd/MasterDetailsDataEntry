using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
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
            using (var db = new AdventureWorksContext())
            {
                var reader = new EntityFrameworkStoreSchemaReader();
                var ps = new EntityFrameworkStoreSchemaReaderParameters { Context = db };

                var schema = reader.ReadSchema(ps);
            }
        }
    }
}
