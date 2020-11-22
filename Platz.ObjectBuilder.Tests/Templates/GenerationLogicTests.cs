using MasterDetailsDataEntry.Demo.Database;
using Platz.ObjectBuilder.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests
{
    public class GenerationLogicTests
    {
        [Fact]
        public void RunGenerationLogicTest()
        {
            var logic = new GenerationLogic();
            logic.Void();
        }

        private void SimpleQuery()
        {
            using (var db = new AdventureWorksContext())
            {
                var query =
                    from c in db.Customer
                    join cu in db.CustomerAddress on c.CustomerId equals cu.CustomerId
                    where c.CustomerId == 1
                    select new 
                    { 
                        CustomerId = c.CustomerId, 
                        FirstName = c.FirstName, 
                        LastName = c.LastName, 
                    };
            }

            using (var db = new AdventureWorksContext())
            {
                var query =
                    from c in db.Customer
                    select new 
                    {
                        CustomerId = c.CustomerId,
                        CompanyName = c.CompanyName,
                        EmailAddress = c.EmailAddress,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        MiddleName = c.MiddleName,
                        ModifiedDate = c.ModifiedDate,
                        NameStyle = c.NameStyle,
                        PasswordHash = c.PasswordHash,
                        PasswordSalt = c.PasswordSalt,
                        Phone = c.Phone,
                        Rowguid = c.Rowguid,
                        SalesPerson = c.SalesPerson,
                        Suffix = c.Suffix,
                        Title = c.Title,
                    };

                var result = query.ToList();
            }
        }
    }
}
