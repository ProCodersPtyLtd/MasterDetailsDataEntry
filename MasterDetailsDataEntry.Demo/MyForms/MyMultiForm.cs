//using MasterDetailsDataEntry.Blazor;
//using MasterDetailsDataEntry.Demo.Data;
//using MasterDetailsDataEntry.Demo.Database.Model;
//using MasterDetailsDataEntry.Shared;
//using MasterDetailsDataEntry.Shared.Forms;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MasterDetailsDataEntry.Demo.MyForms
//{
//    public class MyMultiForm : MasterDetailsForm<Order, OrderItem>
//    {
//        protected override void Define()
//        {
//            this
//                .Use<TestContext>()
//                // order
//                .PrimaryKey(m => m.Id)
//                .MasterField(m => m.Id, new Field { Hidden = true })
//                .MasterField(m => m.ClientName, new Field { Required = true, ControlType = typeof(TextEdit) })
//                .MasterField(m => m.CreateDate, new Field { Required = true })
//                .MasterField(m => m.ExecuteDate, new Field { })
//                // items
//                .ForeignKey(d => d.OrderId)
//                .Field(d => d.Id, new Field { Hidden = true })
//                .Field(d => d.ItemName, new Field { Required = true, ControlType = typeof(TextEdit) })
//                .Field(d => d.Qty, new Field { Required = true, ControlType = typeof(TextEdit) })
//                ;
//        }
//    }
//}
