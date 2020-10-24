using MasterDetailsDataEntry.Blazor;
using MasterDetailsDataEntry.Demo.Data;
using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class MyMultiForm : MasterDetailsForm<Order, OrderItem>
    {
        protected override void Define()
        {
            this
                // order
                .PrimaryKey(m => m.OrderId)
                .MasterField(m => m.OrderId, new Field { Hidden = true })
                .MasterField(m => m.OrderDate, new Field { Required = true })
                .MasterField(m => m.ClientName, new Field { Required = true, ControlType = typeof(TextEdit) })
                // items
                .ForeignKey(d => d.OrderId)
                .Field(d => d.OrderItemId, new Field { Hidden = true })
                .Field(d => d.Name, new Field { Required = true, ControlType = typeof(TextEdit) })
                .Field(d => d.Qty, new Field { Required = true, ControlType = typeof(TextEdit) })
                ;
        }
    }
}
