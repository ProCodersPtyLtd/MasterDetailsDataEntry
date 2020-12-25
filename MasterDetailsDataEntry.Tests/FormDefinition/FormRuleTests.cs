using MasterDetailsDataEntry.Demo.Database.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Platz.SqlForms.Shared;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class FormRuleTests
    {
        [Fact]
        public void FormRuleExecuteTest()
        {
            var form = new TestForm3();
            var fields = ((IDynamicEditForm)form).GetFields();
            Assert.Equal(5, fields.Count());

            var nameField = fields.First(f => f.BindingProperty == "ClientName");
            Assert.Single(nameField.Rules);

            var order = new Order { ClientName = "Bashirov" };
            var result = nameField.Rules[0].Method(order);
            Assert.Null(result);
        }

        [Fact]
        public void FormRuleErrorTest()
        {
            var form = new TestForm3();
            var fields = ((IDynamicEditForm)form).GetFields();
            Assert.Equal(5, fields.Count());

            var nameField = fields.First(f => f.BindingProperty == "ClientName");
            Assert.Single(nameField.Rules);

            var order = new Order { ClientName = "Petrov" };
            var result = nameField.Rules[0].Method(order);
            Assert.True(result.IsFailed);
            Assert.Equal("wrong name", result.Message);
            Assert.Equal("TestForm3", result.RuleName);
        }


        public class TestForm3 : DynamicEditFormBase<TestContext>
        {
            protected override void Define(DynamicFormBuilder builder)
            {
                builder.Entity<Order>(e =>
                {
                    e.Property(e => e.Id).IsRequired().IsHidden().IsReadOnly().Label("PK").Format("d").IsFilter()
                        .Control(typeof(DefaultTextEditControl)).ControlReadOnly(typeof(DefaultReadonlyCheckboxControl));

                    e.Property(e => e.ClientName).Rule(CheckName);
                });
            }

            public FormRuleResult CheckName(Order model)
            {
                if (model.ClientName == "Petrov")
                {
                    return new FormRuleResult("wrong name");
                }

                return null;
            }
        }
    }

    
}
