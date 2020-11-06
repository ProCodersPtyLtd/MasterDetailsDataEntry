using MasterDetailsDataEntry.Demo.Database.Model;
using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class FormBuilderTests
    {
        [Fact]
        public void SimpleFormBuildTest()
        {
            var form = new TestForm1() as IModelDefinitionForm;
            var fields = form.GetDetailsFields().Where(f => f.BindingProperty == "Id");
            Assert.Single(fields);
            var f = fields.Single();
            Assert.True(f.Required);
            Assert.True(f.Hidden);
            Assert.True(f.ReadOnly);
            Assert.True(f.Filter);
            Assert.Equal("PK", f.Label);
            Assert.Equal("d", f.Format);
            Assert.Equal(typeof(int), f.DataType);
            Assert.Equal(typeof(DefaultTextEditControl), f.ControlType);
            Assert.Equal(typeof(DefaultReadonlyCheckboxControl), f.ViewModeControlType);
        }

        [Fact]
        public void AllFieldsIncludedFormBuildTest()
        {
            var form = new TestForm1() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            Assert.Equal(5, fields.Count());
        }

        [Fact]
        public void CanHideFieldFormBuildTest()
        {
            var form = new TestForm1() as IModelDefinitionForm;
            var fields = form.GetDetailsFields().Where(f => !f.Hidden);
            Assert.Equal(4, fields.Count());
        }

        [Fact]
        public void DropDownFormBuildTest()
        {
            var form = new TestForm2() as IModelDefinitionForm;
            var fields = form.GetDetailsFields().Where(f => f.BindingProperty == "ClientId");
            Assert.Single(fields);
            var f = fields.Single();
            Assert.True(f.Required);
            Assert.False(f.Hidden);
            Assert.Null(f.ReadOnly);
            Assert.False(f.Filter);
            Assert.Equal("Client", f.Label);
            Assert.Equal(typeof(Client), f.SelectEntityType);
            Assert.Equal("Id", f.SelectIdProperty);
            Assert.Equal("Name", f.SelectNameProperty);
        }

        [Fact]
        public void ControlsPopulatedFormBuildTest()
        {
            var form = new TestForm1() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();

            Assert.Empty(fields.Where(f => f.ControlType == null));
            Assert.Empty(fields.Where(f => f.ViewModeControlType == null));
        }

        public class TestForm1 : DetailsForm<TestContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<Order>(e =>
                {
                    e.Property(e => e.Id).IsRequired().IsHidden().IsReadOnly().Label("PK").Format("d").IsFilter()
                        .Control(typeof(DefaultTextEditControl)).ControlReadOnly(typeof(DefaultReadonlyCheckboxControl));
                });
            }
        }

        public class TestForm2 : DetailsForm<TestContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<Order>(e =>
                {
                    e.Property(e => e.ClientId).Dropdown<Client>().Set(c => c.Id, c => c.Name).IsRequired().Label("Client");
                });
            }
        }
    }
}
