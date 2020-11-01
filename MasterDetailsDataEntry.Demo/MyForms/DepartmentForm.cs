using MasterDetailsDataEntry.Demo.Database.School;
using MasterDetailsDataEntry.Shared.Forms;
using MasterDetailsDataEntry.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterDetailsDataEntry.Shared.Controls;
using Platz.SqlForms;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class DepartmentForm : DetailsForm<SchoolContext>
    {
        //protected override void Define()
        //{
        //    this
        //        .Use<SchoolContext>()
        //        .PrimaryKey(d => d.DepartmentId) // .Field(d => d.DepartmentId, new Field { ControlType = typeof(DefaultFormattedViewControl) })
        //        .FilterBy(m => m.DepartmentId)
        //        .Field(d=> d.Name, new Field { Required = true })
        //        .Field(d=> d.StartDate, new Field { Required = true })
        //        .Field(d=> d.Budget, new Field { Required = true })
        //        .Dropdown<Person>().Field(m => m.Administrator, c => c.PersonId, c => c.FullName, new Field { Required = true })
        //        ;
        //}
        protected override void Define(FormBuilder builder)
        {
            builder.Entity<Department>(e =>
            {
                e.Property(p => p.DepartmentId).IsFilter();

                e.Property(p => p.Name).IsRequired();

                e.Property(p => p.StartDate).IsRequired();

                e.Property(p => p.Budget).IsRequired();

                e.Property(p => p.Administrator).Dropdown<Person>().Set(c => c.PersonId, c => c.FullName).IsRequired();
            });
        }
    }
}
