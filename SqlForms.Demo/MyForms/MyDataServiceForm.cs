using MasterDetailsDataEntry.Demo.Database.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Platz.SqlForms;
using Default;

namespace SqlForms.Demo.MyForms
{
    public class MyDataServiceForm : MyDataService
    {
        protected override void Define(DataServiceFormBuilder builder)
        {
            builder.Entity<Cust>(e =>
            {
                e.ExcludeAll();

                e.Property(p => p.CustomerId).IsPrimaryKey();

                e.Property(p => p.FirstName);

                e.Property(p => p.LastName);

                e.Property(p => p.Phone);

                e.Property(p => p.EmailAddress);

                e.Property(p => p.CompanyName);

                e.ContextButton("Details", "CustomerView/{0}").ContextButton("Edit", "CustomerEdit/{0}").ContextButton("Addresses", "CustAddrList/{0}")
                    .ContextButton("Delete", "CustomerDelete/{0}");
            });

            builder.SetListMethod(GetCustList);
        }

        //public void 
    }
}
