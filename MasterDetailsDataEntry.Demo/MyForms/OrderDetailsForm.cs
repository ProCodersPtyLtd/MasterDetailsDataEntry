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
    public class OrderDetailsForm : DetailsForm<Order>
    {
        protected override void Define()
        {
            this
                .Use<TestContext>()
                // keys
                .PrimaryKey(m => m.Id)
                // fields
                .Field(m => m.Id, new Field { Hidden = true })
                .Field(m => m.ClientName, new Field { Required = true, ControlType = typeof(TextEdit) })
                .Field(m => m.CreateDate, new Field { Required = true })
                .Field(m => m.ExecuteDate, new Field { })
                .AddDropdown<Client>().Field(m => m.ClientId, c => c.Id, c => c.Name)
                ;
        }
    }
}
