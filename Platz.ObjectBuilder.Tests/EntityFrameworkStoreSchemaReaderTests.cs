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

        [Fact]
        public void Queries()
        {
            using(var db = new AdventureWorksContext())
            {
                var q = from ca in db.CustomerAddress
                        join c in db.Customer on ca.CustomerId equals c.CustomerId
                        group c by new { c.CustomerId, c.FirstName, c.LastName } into cg
                        where cg.Count() > 1
                        select new { cg.Key.FirstName, cg.Key.LastName, Count = cg.Count() };

                var grp = q.ToList();
            }
        }
    }
}
