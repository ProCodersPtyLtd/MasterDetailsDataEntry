using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class MyCustEditForm : DynamicEditFormBase
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<Customer>(e =>
            {
                e.Property(p => p.CustomerId).IsHidden();

                e.Property(p => p.FirstName);

                e.Property(p => p.LastName);

                e.Property(p => p.Phone);

                e.Property(p => p.EmailAddress).Rule(CheckEmail);

                e.Property(p => p.CompanyName);

                e.ContextButton("Details", "CustView/{0}").ContextButton("Edit", "CustEdit/{0}").ContextButton("Delete", "CustDelete/{0}");
            });

            // builder.SetListMethod(GetCustList);
        }

        public FormRuleResult CheckEmail(Customer model)
        {
            if (!model.EmailAddress.Contains("@"))
            {
                return new FormRuleResult("Email shoulc contacin '@' symbol");
            }

            return null;
        }
    }
}
