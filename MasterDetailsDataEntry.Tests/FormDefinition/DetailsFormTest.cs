using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class DetailsFormTest
    {
        [Fact]
        public void ContextFormTest()
        {
            var f = new FormWithContext();
            Assert.Equal(typeof(TestModelsContext), f.GetDbContextType());
        }

        [Fact]
        public void EmptyFormTest()
        {
            var f = new FormWithContext() as IModelDefinitionForm;
            var fields = f.GetDetailsFields();
            Assert.Empty(fields);
        }

        [Fact]
        public void GetDetailsTypeTest()
        {
            var f = new FormWithEntity();
            Assert.Equal(typeof(TestOrder), f.GetDetailsType());
        }

        [Fact]
        public void GetFieldFormatTest()
        {
            var f = new FormWithDates() as IModelDefinitionForm;
            var fields = f.GetDetailsFields();
            Assert.Equal("MMM-yyyy-dd", f.GetFieldFormat("CreateDate"));
        }

        [Fact]
        public void GetDefaultDateFieldFormatTest()
        {
            var f = new FormWithDates() as IModelDefinitionForm;
            var fields = f.GetDetailsFields();
            Assert.Equal("dd/MM/yyyy", f.GetFieldFormat("ExecuteDate"));
        }

        public class FormWithContext : DetailsForm<TestModelsContext>
        {
            protected override void Define(FormBuilder builder)
            {
            }
        }

        public class FormWithEntity : DetailsForm<TestModelsContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<TestOrder>(e =>
                {
                });
            }
        }

        public class FormWithDates : DetailsForm<TestModelsContext>
        {
            protected override void Define(FormBuilder builder)
            {
                builder.Entity<TestOrder>(e =>
                {
                    e.Property(p => p.CreateDate).Format("MMM-yyyy-dd");
                    e.Property(p => p.ExecuteDate).IsRequired();
                });
            }
        }
    }
}
