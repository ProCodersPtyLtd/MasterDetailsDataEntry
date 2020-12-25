using Microsoft.EntityFrameworkCore;
using Platz.SqlForms;
using Platz.SqlForms.Shared;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Platz.SqlForms
{
    public class DataEntryProvider : IDataEntryProvider
    {
        // read fields
        public IEnumerable<DataField> GetFormFields(IModelDefinitionForm form, Type entity = null)
        {
            var fieldsSet = entity == null? form.GetDetailsFields() : form.GetEntityFields(entity);

            using (var db = GetDbContext(form))
            {
                var pk = db.FindSinglePrimaryKeyProperty(entity ?? form.GetDetailsType());
                var pkField = fieldsSet.Single(f => f.BindingProperty == pk.Name);
                pkField.PrimaryKey = true;
                pkField.PrimaryKeyGeneratedType = (PrimaryKeyGeneratedTypes)pk.ValueGenerated;
            }

            return fieldsSet;
        }

        //public IEnumerable<DataField> GetFormFields(Type formType)
        //{
        //    throw new NotImplementedException();
        //}

        //public Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetMultiFormFields(Type formType)
        //{
        //    throw new NotImplementedException();
        //}

        // read data
        //public IList GetModelData(IModelDefinitionForm form)
        //{
        //    throw new NotImplementedException();
        //}

        public IList GetFilteredModelData(IModelDefinitionForm form, int? filterValue, Type entity = null)
        {
            using (var db = GetDbContext(form))
            {
                var entityType = entity ?? form.GetDetailsType();
                IQueryable query = db.FindSet(entityType);

                if (filterValue != null)
                //{
                //    query = query.Where("1 = 2");
                //}
                //else
                {
                    var filterColumn = form.GetDetailsFields().Single(f => f.Filter).BindingProperty;
                    query = query.Where($"{filterColumn} = {filterValue}");
                }

                var result = query.ToDynamicList<object>();
                return result;
            }
        }

        //public object GetModelData(IModelDefinitionForm form, int Id)
        //{
        //    throw new NotImplementedException();
        //}

        public System.Collections.IList GetEntityData(IDataForm form, Type entity)
        {
            using (var db = GetDbContext(form))
            {
                var result = db.FindSet(entity).ToDynamicList<object>();
                return result;
            }
        }

        public bool IsPropertyValueNotUnique(IModelDefinitionForm form, object item, string bindingProperty, Type entity)
        {
            using (var db = GetDbContext(form))
            {
                var value = item.GetPropertyValue(bindingProperty);
                var result = db.FindSet(entity).Any($"{bindingProperty} = {value.ToSql()}");
                return result;
            }
        }

        // CRUD
        public void DeleteItem(IModelDefinitionForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Remove(item);
                db.SaveChanges();
            }
        }

        public object InsertItem(IModelDefinitionForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Add(item);
                db.SaveChanges();
                return item;
            }
        }

        public object UpdateItem(IModelDefinitionForm form, object item)
        {
            using (var db = GetDbContext(form))
            {
                db.Update(item);
                db.SaveChanges();
                return item;
            }
        }

        private DbContext GetDbContext(IDataForm form)
        {
            var type = form.GetDbContextType();
            var context = Activator.CreateInstance(type) as DbContext;
            return context;
        }
    }
}
