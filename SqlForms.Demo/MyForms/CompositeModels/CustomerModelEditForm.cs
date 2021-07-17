using MasterDetailsDataEntry.Demo.Database;
using MasterDetailsDataEntry.Demo.Database.AdventureWorks;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.Demo.MyForms
{
    /// <summary>
    /// dev example - not used 
    /// </summary>
    public class CustomerModelEditForm : DynamicEditFormBase<AdventureWorksContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            //(new AdventureWorksContext()).Find();

            builder.Entity<CustomerModel>(e =>
            {
                // Load/Save
                //e.Property(p => p.Customer).Get(db => db.Find).Set();
                e.Context(p => p.Customer).Load().Save();

                // mapping CustomerId => r.AddressId just example, how to map model property to property in the list
                e.ContextPropertyList(p => p.Addresses).Load(GetAddressList).Save(SaveAddressList).SavingMapping(m => m.Customer.CustomerId, r => r.AddressId); ;

                e.Context(p => p.OrderHeader).Load().Save().SavingMapping(m => m.Customer.CustomerId, r => r.CustomerId);

                // UI
                e.Property(p => p.Customer.CustomerId).IsReadOnly();

                e.Property(p => p.Customer.FirstName).IsRequired();

                e.Property(p => p.Customer.LastName).IsRequired();

                e.Property(p => p.Customer.Phone);

                e.Property(p => p.Customer.EmailAddress);

                e.Property(p => p.Customer.CompanyName);

                // Buttons
                e
                //.DialogButton("QueryResult", ButtonActionTypes.Custom, "Back")
                .DialogButton("CustAddrCountList", ButtonActionTypes.Cancel)
                //.DialogButton(ButtonActionTypes.Close)
                .DialogButton(ButtonActionTypes.Validate)
                .DialogButton(ButtonActionTypes.Submit, "Update", "Save to database");

                e.DialogButtonNavigation("CustAddrCountList", ButtonActionTypes.Delete, ButtonActionTypes.Submit);

                e.DialogButtonNavigation("/", ButtonActionTypes.Close);
            });

        }

        //public List<Address> GetAddressList(CustomerModel model)
        //{
        //    throw new NotImplementedException();
        //}

        //public void SaveAddressList(CustomerModel model)
        //{
        //    throw new NotImplementedException();
        //}

        public List<Address> GetAddressList(CustomerModel model, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void SaveAddressList(CustomerModel model, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomerModel
    {
        public Customer Customer { get; set; }
        public SalesOrderHeader OrderHeader { get; set; }
        public List<Address> Addresses { get; set; }
    }
}
