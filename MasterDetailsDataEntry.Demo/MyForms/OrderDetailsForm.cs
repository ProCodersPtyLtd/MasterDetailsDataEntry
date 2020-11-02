using MasterDetailsDataEntry.Demo.Database.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class OrderDetailsForm : DetailsForm<TestContext>
    {
        protected override void Define(FormBuilder builder)
        {
            builder.Entity<Order>(e =>
            {
                e.Property(p => p.Id).IsHidden();

                e.Property(p => p.ClientName).IsRequired();

                e.Property(p => p.CreateDate).IsRequired();
                e.Property(p => p.ExecuteDate);

                e.Property(p => p.ClientId).Dropdown<Client>().Set(c => c.Id, c => c.Name).IsRequired();
            });
        }
    }
}
