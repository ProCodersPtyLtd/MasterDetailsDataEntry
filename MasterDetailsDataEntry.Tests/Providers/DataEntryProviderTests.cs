using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Demo.Database.School;
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

        [Fact]
        public void ValueGeneratedPkTest()
        {
            var provider = new DataEntryProvider();
            var fields = provider.GetFormFields(new TestForm1()).Where(f => f.BindingProperty == "Id");
            Assert.Single(fields);
            var f = fields.Single();
            Assert.Equal(PrimaryKeyGeneratedTypes.OnAdd, f.PrimaryKeyGeneratedType);
        }

        [Fact]
        public void ValueGeneratedNoIdentityPkTest()
        {
            var provider = new DataEntryProvider();
            var fields = provider.GetFormFields(new SchoolTestForm1()).Where(f => f.BindingProperty == "DepartmentId");
            Assert.Single(fields);
            var f = fields.Single();
            Assert.Equal( PrimaryKeyGeneratedTypes.Never, f.PrimaryKeyGeneratedType);
        }

        public class TestForm1 : DetailsForm<TestContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<Order>(entity =>
                {
                    entity.Property(e => e.Id).IsRequired().IsHidden().IsReadOnly().Label("PK").Format("d");
                });
            }
        }

        public class SchoolTestForm1 : DetailsForm<SchoolContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<Department>(entity =>
                {
                    entity.Property(e => e.DepartmentId).IsRequired().IsHidden().IsReadOnly().Label("PK").Format("d");
                });
            }
        }
    }
}
