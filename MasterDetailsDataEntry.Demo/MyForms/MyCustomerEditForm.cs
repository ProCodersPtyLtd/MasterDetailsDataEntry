using MasterDetailsDataEntry.Demo.Database;
using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class MyCustomerEditForm : DynamicEditFormBase<AdventureWorksContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<Customer>(e =>
            {
                e.Property(p => p.CustomerId).IsReadOnly();

                e.Property(p => p.FirstName).IsRequired();

                e.Property(p => p.LastName);

                e.Property(p => p.Phone);

                e.Property(p => p.EmailAddress).Rule(CheckEmail);

                e.Property(p => p.CompanyName);

                e.DialogButton("QueryResult", ButtonActionTypes.Custom, "Back")
                .DialogButton("QueryResult", ButtonActionTypes.Cancel)
                .DialogButton("/", ButtonActionTypes.Close)
                .DialogButton(ButtonActionTypes.Validate)
                .DialogButton("QueryResult", ButtonActionTypes.Submit, "Update", "Save to database");
            });

        }

        public Customer GetById(int id)
        {
            return null;
        }
        public Customer Save(Customer model)
        {
            return null;
        }


        public FormRuleResult CheckEmail(Customer model)
        {
            if (model.EmailAddress != null && !model.EmailAddress.Contains("@"))
            {
                return new FormRuleResult("Email should contain '@' symbol");
            }

            return null;
        }
    }
}
