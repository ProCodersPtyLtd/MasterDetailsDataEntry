using Platz.SqlForms;
using SqlForms.Demo.Database.AdventureWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.SqlForms.Forms
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
