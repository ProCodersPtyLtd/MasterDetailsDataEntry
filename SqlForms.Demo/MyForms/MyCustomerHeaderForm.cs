using MasterDetailsDataEntry.Demo.Database;
using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.Demo.MyForms
{
    public class MyCustomerHeaderForm : DynamicEditFormBase<AdventureWorksContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<Customer>(e =>
            {
                e.ExcludeAll();

                e.Property(p => p.CustomerId).IsReadOnly();

                e.Property(p => p.FirstName).IsReadOnly();

                e.Property(p => p.MiddleName).IsReadOnly();

                e.Property(p => p.LastName).IsReadOnly();
            });
        }
    }
}
