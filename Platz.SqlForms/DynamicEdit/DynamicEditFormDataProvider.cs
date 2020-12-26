using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
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

        // CRUD
        public object GetItem(IDynamicEditForm form, int id)
        {
            // ToDo: check is form overrides GetItem operation
            Type entity = form.GetEntityType();

            using (var db = GetDbContext(form))
            {
                var item = db.Find(entity, id);
                return item;
            }
        }

        public void DeleteItem(IDynamicEditForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Remove(item);
                db.SaveChanges();
            }
        }

        public object InsertItem(IDynamicEditForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Add(item);
                db.SaveChanges();
                return item;
            }
        }

        public object UpdateItem(IDynamicEditForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Update(item);
                db.SaveChanges();
                return item;
            }
        }

        private DbContext GetDbContext(IDynamicEditForm form)
        {
            var type = form.GetDbContextType();
            var context = Activator.CreateInstance(type) as DbContext;
            return context;
        }
    }
}
