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
        public IEnumerable<DataField> GetFormFields(IModelDefinitionForm form)
        {
            var fieldsSet = form.GetDetailsFields();

            using (var db = GetDbContext(form))
            {
                var pk = db.FindSinglePrimaryKeyProperty(form.GetDetailsType());
                var pkField = fieldsSet.Single(f => f.BindingProperty == pk.Name);
                pkField.PrimaryKey = true;
                pkField.PrimaryKeyGeneratedType = (PrimaryKeyGeneratedTypes)pk.ValueGenerated;

                //if (pkField.PrimaryKeyGeneratedType == PrimaryKeyGeneratedTypes.Never)
                //{
                //    pkField.Required = true;
                //}
                // set readonly based on PrimaryKeyGeneratedType
                //if (pkField.ReadOnly == null)
                //{
                //    pkField.ReadOnly = pkField.PrimaryKeyGeneratedType == PrimaryKeyGeneratedTypes.OnAdd;
                //}

                //if (pkField.ReadOnly == false && !pkField.Required)
                //{
                //    pkField.Required = true;
                //}
            }

            return fieldsSet;
        }

        public IEnumerable<DataField> GetFormFields(Type formType)
        {
            throw new NotImplementedException();
        }

        public Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetMultiFormFields(Type formType)
        {
            throw new NotImplementedException();
        }

        // read data
        public IList GetModelData(IModelDefinitionForm form)
        {
            throw new NotImplementedException();
        }

        public IList GetFilteredModelData(IModelDefinitionForm form, int? filterValue)
        {
            using (var db = GetDbContext(form))
            {
                var detailsType = form.GetDetailsType();
                IQueryable query = db.FindSet(detailsType);

                if (filterValue != null)
                {
                    var filterColumn = form.GetDetailsFields().Single(f => f.Filter).BindingProperty;
                    query = query.Where($"{filterColumn} = {filterValue}");
                }

                var result = query.ToDynamicList<object>();
                return result;
            }
        }

        public object GetModelData(IModelDefinitionForm form, int Id)
        {
            throw new NotImplementedException();
        }

        public System.Collections.IList GetEntityData(IModelDefinitionForm form, Type entity)
        {
            using (var db = GetDbContext(form))
            {
                var result = db.FindSet(entity).ToDynamicList<object>();
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

        private DbContext GetDbContext(IModelDefinitionForm form)
        {
            var type = form.GetDbContextType();
            var context = Activator.CreateInstance(type) as DbContext;
            return context;
        }
    }
}
