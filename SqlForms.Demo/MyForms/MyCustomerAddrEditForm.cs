using MasterDetailsDataEntry.Demo.Database;
using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.Demo.MyForms
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

                e.Property(p => p.CountryRegion).IsRequired();

                e.Property(p => p.PostalCode).IsRequired();

                e.Property(p => p.ModifiedDate).IsReadOnly().Rule(DefaultModifiedDate, FormRuleTriggers.Create);

                e.DialogButton(ButtonActionTypes.Cancel).DialogButton(ButtonActionTypes.Validate).DialogButton(ButtonActionTypes.Submit);

                e.DialogButtonNavigation("CustAddrList/{1}", ButtonActionTypes.Delete, ButtonActionTypes.Cancel, ButtonActionTypes.Submit);

                e.AfterSave(SaveToCustomerAddress);
            });
        }

        public void SaveToCustomerAddress(Address model, DataOperationArgs args)
        {
            if (args.Operation == DataOperations.Insert)
            {
                int customerId = (int)args.Parameters[0];
                var item = new CustomerAddress { AddressId = model.AddressId, CustomerId = customerId, ModifiedDate = DateTime.Now, AddressType = "Test" };
                args.DbContext.Add(item);
                args.DbContext.SaveChanges();
            }
        }

        public FormRuleResult DefaultModifiedDate(Address model)
        {
            model.ModifiedDate = DateTime.Now;
            return null;
        }
    }
}
