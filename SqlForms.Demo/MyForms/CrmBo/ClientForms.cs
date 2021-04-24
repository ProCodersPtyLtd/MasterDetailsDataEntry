using CrmServices;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.Demo.MyForms.CrmBo
{
    public class ClientEditForm : StoreDynamicEditFormBase<CrmBoContext>
    {
        protected override void Define(DynamicFormBuilder builder)
        {
            builder.Entity<ClientModel>(e =>
            {
                e.Context(p => p.Client).Load().Save();
                e.Context(p => p.ClientContact).Load().Save().SavingMapping(p => p.Client.Id, p => p.PersonId);
                e.ContextPropertyList(p => p.ClientAddressList).Load().Save().SavingMapping(p => p.Client.Id, p => p.PersonId);

                e.Property(p => p.Client.Id).IsReadOnly();
                e.Property(p => p.Client.FirstName).IsRequired();
                e.Property(p => p.Client.LastName).IsRequired();

                //e.DialogButton(ButtonActionTypes.Cancel).DialogButton(ButtonActionTypes.Validate).DialogButton(ButtonActionTypes.Submit);
                //e.DialogButtonNavigation("OrderList", ButtonActionTypes.Delete, ButtonActionTypes.Cancel, ButtonActionTypes.Submit);
            });
        }
    }
}
