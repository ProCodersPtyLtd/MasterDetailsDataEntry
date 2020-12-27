using MasterDetailsDataEntry.Demo.Database;
using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class MyCustomerAddrEditForm : DynamicEditFormBase<AdventureWorksContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<Address>(e =>
            {
                e.Property(p => p.AddressId).IsReadOnly();

                e.Property(p => p.AddressLine1).IsRequired();

                e.Property(p => p.AddressLine2);

                e.Property(p => p.City).IsRequired();

                e.Property(p => p.CountryRegion);

                e.Property(p => p.PostalCode).IsRequired();

                e
                .DialogButton(ButtonActionTypes.Cancel)
                .DialogButton(ButtonActionTypes.Validate)
                .DialogButton(ButtonActionTypes.Submit);

                e.DialogButtonNavigation("QueryResult", ButtonActionTypes.Delete, ButtonActionTypes.Submit);

                e.DialogButtonNavigation("CustAddrList/{1}", ButtonActionTypes.Cancel);

                e.AfterSave(SaveToCustomerAddress);
            });
        }

        public void SaveToCustomerAddress(Address model, DataOperationArgs args)
        {
            int customerId = (int)args.Parameters[0];
            var item = new CustomerAddress { AddressId = model.AddressId, CustomerId = customerId, ModifiedDate = DateTime.Now };
            args.DbContext.Add(item);
            args.DbContext.SaveChanges();
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
