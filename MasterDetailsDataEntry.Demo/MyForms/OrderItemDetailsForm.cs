using MasterDetailsDataEntry.Blazor;
using MasterDetailsDataEntry.Demo.Data;
using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Controls;
using MasterDetailsDataEntry.Shared.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class OrderItemDetailsForm : DetailsForm<OrderItem>
    {
        protected override void Define()
        {
            this
                .Use<TestContext>()
                // keys
                .PrimaryKey(m => m.Id)
                .FilterBy(m => m.OrderId)
                .ForeignKey(d => d.OrderId)
                // fields
                .Field(d => d.Id, new Field { ControlType = typeof(DefaultFormattedViewControl) })
                .Field(d => d.OrderId, new Field { ControlType = typeof(DefaultFormattedViewControl) })
                .Field(d => d.ItemName, new Field { Required = true, ControlType = typeof(TextEdit) })
                .Field(d => d.Qty, new Field { Required = true, ControlType = typeof(TextEdit) })
                .Field(d => d.IsMajor, new Field { })
                .Field(d => d.AvailableFrom, new Field { Required = true })
                ;
        }
    }
}
