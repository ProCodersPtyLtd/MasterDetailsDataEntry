using MasterDetailsDataEntry.Shared.Controls;
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

        [Fact]
        public void DefineDropdownTest()
        {
            var f = new FormWithDropdown();
            var fields = f.GetFields();
            Assert.NotEmpty(fields);
            Assert.Contains(fields, x => x.SelectEntityType != null);
            var df = fields.Single(x => x.SelectEntityType != null);
            Assert.Equal(typeof(TestClient), df.SelectEntityType);
            Assert.Equal("Id", df.SelectIdProperty);
            Assert.Equal("Name", df.SelectNameProperty);
            Assert.Equal(typeof(DefaultDropdownControl), df.ControlType);
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

        public class FormWithDropdown : DetailsForm<TestOrder>
        {
            protected override void Define()
            {
                this
                    .Use<TestModelsContext>()
                    .PrimaryKey(x => x.Id)
                    .AddDropdown<TestClient>().Field(o => o.ClientId, c => c.Id, c => c.Name)
                    .Field(o => o.CreateDate, new Shared.Field { Hidden = true })
                    ;
            }
        }
        #endregion
    }


}
