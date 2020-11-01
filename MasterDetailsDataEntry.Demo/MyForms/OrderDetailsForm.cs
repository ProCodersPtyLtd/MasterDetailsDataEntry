using MasterDetailsDataEntry.Blazor;
using MasterDetailsDataEntry.Demo.Data;
using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Controls;
using MasterDetailsDataEntry.Shared.Forms;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class OrderDetailsForm : DetailsForm<TestContext>
    {
        //protected override void Define()
        //{
        //    this
        //        .Use<TestContext>()
        //        // keys
        //        .PrimaryKey(m => m.Id)
        //        // fields
        //        .Field(m => m.Id, new Field { Hidden = true })
        //        .Field(m => m.ClientName, new Field { Required = true, ControlType = typeof(TextEdit) })
        //        .Field(m => m.CreateDate, new Field { Required = true })
        //        .Field(m => m.ExecuteDate, new Field { })
        //        .Dropdown<Client>().Field(m => m.ClientId, c => c.Id, c => c.Name, new Field { Required = true })
        //        ;
        //}
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
