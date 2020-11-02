using MasterDetailsDataEntry.Demo.Database.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class OrderItemDetailsForm : DetailsForm<TestContext>
    {
        protected override void Define(FormBuilder builder) 
        {
            builder.Entity<OrderItem>(e =>
            {
                // e.Property(p => p.Id).Control(typeof(DefaultFormattedViewControl));

                e.Property(p => p.OrderId).IsFilter().Control(typeof(DefaultFormattedViewControl));

                e.Property(p => p.ItemName).IsRequired();
                e.Property(p => p.Qty).IsRequired();
                e.Property(p => p.Qty2).IsRequired();
                e.Property(p => p.Price).IsRequired();

                e.Property(p => p.IsMajor);
                e.Property(p => p.AvailableFrom).IsRequired();
            });
        }
    }
}
