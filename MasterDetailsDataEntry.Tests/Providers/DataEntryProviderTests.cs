using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Shared.Forms;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MasterDetailsDataEntry.Tests.Providers
{
    public class DataEntryProviderTests
    {
        [Fact]
        public void FormBuildReadPkTest()
        {
            var provider = new DataEntryProvider();
            var fields = provider.GetFormFields(new TestForm1()).Where(f => f.BindingProperty == "Id");
            Assert.Single(fields);
            var f = fields.Single();
            Assert.True(f.PrimaryKey);
        }

        public class TestForm1 : DetailsForm2<TestContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<Order>(entity =>
                {
                    entity.Property(e => e.Id).IsRequired().IsHidden().IsReadOnly().Label("PK").Format("d");
                });
            }
        }
    }
}
