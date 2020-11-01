using MasterDetailsDataEntry.Shared.Forms;
using MasterDetailsDataEntry.Tests.FormDefinition;
using MasterDetailsDataEntry.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Platz.SqlForms;

namespace MasterDetailsDataEntry.Tests.Providers
{
    public class DataValidationProviderTests
    {
        [Fact]
        public void RequiredValidationForEmptyStringTest()
        {
            var validProvider = new DataValidationProvider();
            var form = new FormWithRequiredField() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var modelItem = new TestOrderItem { };

            var validations = validProvider.ValidateModel(modelItem, 0, fields);
            Assert.Single(validations);
            Assert.Equal(ValidationResultTypes.Error, validations.First().ValidationResultType);
            Assert.Equal("ItemName", validations.First().BindingProperty);
            Assert.Equal("Required", validations.First().Message);
        }

        [Fact]
        public void RequiredValidationForNonEmptyStringTest()
        {
            var validProvider = new DataValidationProvider();
            var form = new FormWithRequiredField() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var modelItem = new TestOrderItem { ItemName = "qq" };

            var validations = validProvider.ValidateModel(modelItem, 0, fields);
            Assert.Empty(validations);
        }

        [Fact]
        public void RequiredPropertyValidationForEmptyStringTest()
        {
            var validProvider = new DataValidationProvider();
            var form = new FormWithRequiredField() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var modelItem = new TestOrderItem { };

            var validations = validProvider.ValidateModelProperty(modelItem, 0, "ItemName", fields);
            Assert.Single(validations);
            Assert.Equal(ValidationResultTypes.Error, validations.First().ValidationResultType);
            Assert.Equal("ItemName", validations.First().BindingProperty);
            Assert.Equal("Required", validations.First().Message);
        }

        public class FormWithRequiredField : DetailsForm<TestModelsContext>
        {
            //protected override void Define()
            //{
            //    this
            //        .Use<TestModelsContext>()
            //        .PrimaryKey(x => x.Id)
            //        .Field(o => o.ItemName, new Shared.Field { Required = true })
            //        ;
            //}

            protected override void Define(FormBuilder builder)
            {
                builder.Entity<TestOrderItem>(e =>
                {
                    e.Property(p => p.ItemName).IsRequired();
                });
            }
        }
    }
}
