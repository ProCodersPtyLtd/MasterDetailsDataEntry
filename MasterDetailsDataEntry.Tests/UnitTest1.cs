using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Forms;
using System;
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
