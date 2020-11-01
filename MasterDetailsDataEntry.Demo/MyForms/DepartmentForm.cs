using MasterDetailsDataEntry.Demo.Database.School;
using MasterDetailsDataEntry.Shared.Forms;
using MasterDetailsDataEntry.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterDetailsDataEntry.Shared.Controls;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class DepartmentForm : DetailsFormOld<Department>
    {
        protected override void Define()
        {
            this
                .Use<SchoolContext>()
                .PrimaryKey(d => d.DepartmentId) // .Field(d => d.DepartmentId, new Field { ControlType = typeof(DefaultFormattedViewControl) })
                .FilterBy(m => m.DepartmentId)
                .Field(d=> d.Name, new Field { Required = true })
                .Field(d=> d.StartDate, new Field { Required = true })
                .Field(d=> d.Budget, new Field { Required = true })
                .Dropdown<Person>().Field(m => m.Administrator, c => c.PersonId, c => c.FullName, new Field { Required = true })
                ;
        }
    }
}
