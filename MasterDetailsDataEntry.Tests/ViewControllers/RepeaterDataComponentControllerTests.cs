using MasterDetailsDataEntry.Tests.FormDefinition;
using Platz.SqlForms.Blazor;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static MasterDetailsDataEntry.Tests.FormDefinition.DetailsFormTest;
using Moq;
using Platz.SqlForms;
using System.Linq;

namespace MasterDetailsDataEntry.Tests.ViewControllers
{
    public class RepeaterDataComponentControllerTests
    {
        [Fact]
        public void CreateGetFieldState()
        {
            var form = new FormWithDates() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var items = new List<TestOrder>();
            items.Add(new TestOrder { Id = 1, ClientId = 1, CreateDate = DateTime.Now });

            var cont = new RepeaterDataComponentController(new Mock<IDataValidationProvider>().Object);
            cont.SetParameters(form, new FormViewOptions { }, form.GetDetailsType(), fields, items);

            var state = cont.CreateFieldState(fields.First(), 0);
            state.IsEditing = true;
            var readState = cont.CreateFieldState(fields.First(), 0);

            Assert.Equal(state, readState);
            Assert.True(readState.IsEditing);
        }

        [Fact]
        public void SetPrimaryKeyIsNewTest()
        {
            var form = new FormWithDates() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var items = new List<TestOrder>();
            items.Add(new TestOrder { Id = 1, ClientId = 1, CreateDate = DateTime.Now });

            var cont = new RepeaterDataComponentController(new Mock<IDataValidationProvider>().Object);
            cont.SetParameters(form, new FormViewOptions { }, form.GetDetailsType(), fields, items);
            fields.First().PrimaryKey = true;
            var state = cont.CreateFieldState(fields.First(), 0);
            Assert.False(state.IsNew);

            cont.SetPrimaryKeyIsNew(0, true);
            Assert.True(state.IsNew);
        }
        
        [Fact]
        public void SetItemValueTest()
        {
            var form = new FormWithDates() as IModelDefinitionForm;
            var fields = form.GetDetailsFields();
            var items = new List<TestOrder>();
            items.Add(new TestOrder { Id = 1, ClientId = 1, CreateDate = DateTime.Now });

            var cont = new RepeaterDataComponentController(new Mock<IDataValidationProvider>().Object);
            cont.SetParameters(form, new FormViewOptions { }, form.GetDetailsType(), fields, items);
            var field = fields.First();
            var state = cont.CreateFieldState(fields.First(), 0);
            state.Value = 17;
            cont.SetItemValue(field, state);
            var value = cont.GetItemValue(field, 0);

            Assert.Equal(17, value);
        }
    }
}
