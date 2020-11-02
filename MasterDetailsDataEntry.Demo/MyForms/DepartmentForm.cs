using MasterDetailsDataEntry.Demo.Database.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Platz.SqlForms;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class DepartmentForm : DetailsForm<SchoolContext>
    {
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
