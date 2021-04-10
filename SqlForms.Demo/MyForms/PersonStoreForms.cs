using Default;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.Demo.MyForms
{
    public class PersonStoreForm : StoreDynamicEditFormBase<CrmDataContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<Person>(e =>
            {
                e.Property(p => p.Id).IsReadOnly();

                e.Property(p => p.Name).IsRequired();

                e.Property(p => p.Surname).IsRequired();

                e.Property(p => p.Dob);

                e.Property(p => p.Phone).IsRequired();

                e.Property(p => p.PrimaryAddressId).Dropdown<Address>().Set(c => c.Id, c => c.Line1).IsRequired();

                e.DialogButton(ButtonActionTypes.Cancel).DialogButton(ButtonActionTypes.Validate).DialogButton(ButtonActionTypes.Submit);

                e.DialogButtonNavigation("PersonStoreList/{0}", ButtonActionTypes.Delete, ButtonActionTypes.Cancel, ButtonActionTypes.Submit);
            });
        }
    }

    public class PersonStoreListForm : StoreDataServiceBase<CrmDataContext>
    {
        protected override void Define(DataServiceFormBuilder builder)
        {
            builder.Entity<Person>(e =>
            {
                e.ExcludeAll();

                e.Property(p => p.Id).IsPrimaryKey();
                e.Property(p => p.Name);
                e.Property(p => p.Surname);
                e.Property(p => p.Dob);
                e.Property(p => p.Phone);
                
                // Parameter {0} is always PrimaryKey, parameters {1} and above - Filter Keys
                // {0} = AddressId {1} = CustomerId
                e.ContextButton("Edit", "PersonStoreEdit/{0}").ContextButton("Delete", "PersonStoreDelete/{0}");

                e.DialogButton("PersonStoreEdit/0", ButtonActionTypes.Add);
            });

            builder.SetListMethod(GetPersonList);
        }

        public List<Person> GetPersonList(params object[] parameters)
        {
            var db = GetDbContext();
            var result = db.Get(typeof(Person)).Cast<Person>().ToList();
            return result;
        }
    }

}
