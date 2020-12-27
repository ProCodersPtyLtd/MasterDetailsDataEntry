using MasterDetailsDataEntry.Demo.Database.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Platz.SqlForms;
using Default;

namespace MasterDetailsDataEntry.Demo.MyForms
{
    public class MyCustomerAddrListForm : MyDataService
    {
        protected override void Define(DataServiceFormBuilder builder)
        {
            builder.Entity<CustomerAddress>(e =>
            {
                e.ExcludeAll();

                e.Property(p => p.AddressId).IsPrimaryKey();

                e.Property(p => p.CustomerId).IsFilter().IsReadOnly();

                e.Property(p => p.Title);

                e.Property(p => p.FirstName);

                e.Property(p => p.LastName);

                e.Property(p => p.AddressLine1);

                e.Property(p => p.AddressLine2);

                e.Property(p => p.City);

                e.Property(p => p.CountryRegion);

                e.Property(p => p.PostalCode);

                // Parameter {0} is always PrimaryKey, parameters {1} and above - Filter Keys
                // {0} = AddressId {1} = CustomerId
                e.ContextButton("Edit", "CustomerAddrEdit/{0}/{1}").ContextButton("Delete", "CustomerAddrDelete/{0}/{1}");

                e.DialogButton("CustomerAddrEdit/0/{1}", ButtonActionTypes.Add);
            });

            builder.SetListMethod(GetCustomerAddressList);
        }
    }
}
