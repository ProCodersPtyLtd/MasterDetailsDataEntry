using MasterDetailsDataEntry.Shared.Forms;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MasterDetailsDataEntry.Tests.FormDefinition
{
    public class DetailsFormTests
    {
        [Fact]
        public void EmptyFormTest()
        {
            var f = new FormEmpty();
            Assert.Null(f.GetDbContextType());
            var fields = f.GetFields();
            Assert.Empty(fields);
        }

        [Fact]
        public void ContextFormTest()
        {
            var f = new FormWithContext();
            Assert.Equal(typeof(TestModelsContext), f.GetDbContextType());
        }

        [Fact]
        public void FormWithPkTest()
        {
            var f = new FormWithPk();
            var fields = f.GetFields();
            Assert.NotEmpty(fields);
            Assert.Contains(fields, x => x.PrimaryKey);
            Assert.Equal("Id", fields.FirstOrDefault(x => x.PrimaryKey)?.BindingProperty);
        }

        #region test forms
        public class FormEmpty: DetailsForm<TestOrder>
        {
            protected override void Define()
            {
            }
        }
        public class FormWithContext : DetailsForm<TestOrder>
        {
            protected override void Define()
            {
                this
                    .Use<TestModelsContext>()
                    ;
            }
        }

        public class FormWithPk : DetailsForm<TestOrder>
        {
            protected override void Define()
            {
                this
                    .Use<TestModelsContext>()
                    .PrimaryKey(x => x.Id)
                    ;
            }
        }
        #endregion
    }


}
