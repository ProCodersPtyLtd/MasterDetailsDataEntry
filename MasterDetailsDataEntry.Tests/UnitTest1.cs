using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Forms;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MasterDetailsDataEntry.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var n = typeof(DateTime?).FullName;
            var n2 = typeof(DateTime?).Name;
            var n3 = typeof(bool).Name;
            var form = new MyForm();
            var f = form.GetFields();

            var provider = new DataEntryProvider();
            var r = provider.GetFormFields(new MyForm());
        }

        [Fact]
        public void DbAccessTest()
        {
            using (var db = new TestContext())
            {
                object order = new Order { ClientName = "test", CreateDate = DateTime.Now, ExecuteDate = DateTime.Now };
                db.Add(order);
                db.SaveChanges();
            }
        }

        [Fact]
        public void DbAccessReadTest()
        {
            var type = typeof(Order);
            var type2 = typeof(OrderItem);

            using (var db = new TestContext())
            {
                var obj = db.Find(type, 1);
                var navigation = db.Entry(obj).Navigations.FirstOrDefault(n => n.Metadata.Name == type2.Name);
                navigation?.Load();
                var l2 = navigation?.CurrentValue as IEnumerable;
                //var l3 = new List<object>();
                //l3.AddRange()
                IList items = l2.Cast<object>().ToList();


                var list = (obj as Order).OrderItem.ToList();
            }
        }
    }

    public class MyForm : MasterDetailsForm<A, B>
    {
        protected override void Define()
        {
            this
                .PrimaryKey(m => m.Id)
                .ForeignKey(d => d.MasterId)
                .MasterField(m => m.Id, new Field { Hidden = true })
                .MasterField(m => m.OrderDate, new Field { Required = true })
                .Field(d => d.MasterId, new Field { Hidden = true })
                ;
        }
    }

    public class A
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class B
    {
        public int MasterId { get; set; }
    }
}
