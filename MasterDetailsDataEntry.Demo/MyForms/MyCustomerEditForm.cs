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
                e.Property(p => p.CustomerId).IsHidden();

                e.Property(p => p.FirstName);

                e.Property(p => p.LastName);

                e.Property(p => p.Phone);

                e.Property(p => p.EmailAddress).Rule(CheckEmail);

                e.Property(p => p.CompanyName);

                //e.ContextButton("Details", "CustView/{0}").ContextButton("Edit", "CustEdit/{0}").ContextButton("Delete", "CustDelete/{0}");

                e.DialogButton("QueryResult", ButtonActionTypes.Custom, "Back")
                .DialogButton("QueryResult", ButtonActionTypes.Cancel)
                .DialogButton("/", ButtonActionTypes.Close)
                .DialogButton(ButtonActionTypes.Validate)
                .DialogButton(ButtonActionTypes.Submit, "Update", "Save to database");
            });

            // builder.GetMethod(GetById);
            // builder.SaveMethod(Save);
            // builder.SetListMethod(GetCustList);
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
            if (!model.EmailAddress.Contains("@"))
            {
                return new FormRuleResult("Email shoulc contacin '@' symbol");
            }

            return null;
        }
    }
}
