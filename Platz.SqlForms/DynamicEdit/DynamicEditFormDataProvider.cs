using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DynamicEditFormDataProvider : IDynamicEditFormDataProvider
    {
        public IEnumerable<DataField> GetFormFields(IDynamicEditForm form)
        {
            var fieldSet = form.GetFields();

            using (var db = GetDbContext(form))
            {
                var pk = db.FindSinglePrimaryKeyProperty(form.GetEntityType());
                var pkField = fieldSet.Single(f => f.BindingProperty == pk.Name);
                pkField.PrimaryKey = true;
                pkField.PrimaryKeyGeneratedType = (PrimaryKeyGeneratedTypes)pk.ValueGenerated;
            }

            return fieldSet;
        }

        private DbContext GetDbContext(IDynamicEditForm form)
        {
            var type = form.GetDbContextType();
            var context = Activator.CreateInstance(type) as DbContext;
            return context;
        }
    }
}
