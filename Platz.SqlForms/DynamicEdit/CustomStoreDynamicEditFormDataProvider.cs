﻿using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Platz.SqlForms
{
    public class CustomStoreDynamicEditFormDataProvider : IDynamicEditFormDataProvider
    {
        private object[] _serviceParameters;

        public void SetParameters(object[] serviceParameters)
        {
            _serviceParameters = serviceParameters;
        }

        public IEnumerable<DataField> GetFormFields(IDynamicEditForm form)
        {
            var fieldSet = form.GetFields();
            var db = GetDbContext(form);
            var pk = db.FindPrimaryKey(form.GetEntityType()).First();
            var pkField = fieldSet.Single(f => f.BindingProperty == pk.Name);
            pkField.PrimaryKey = true;
            pkField.PrimaryKeyGeneratedType = PrimaryKeyGeneratedTypes.OnAdd;
            return fieldSet;
        }

        // CRUD
        public object GetItem(IDynamicEditForm form, int id)
        {
            // ToDo: check is form overrides GetItem operation
            Type entity = form.GetEntityType();
            var db = GetDbContext(form);
            var item = db.Find(entity, id);
            return item;
        }

        public void DeleteItem(IDynamicEditForm form, object item)
        {
            var db = GetDbContext(form);
            db.Delete(item);
        }

        public object InsertItem(IDynamicEditForm form, object item)
        {
            var db = GetDbContext(form);
            db.Insert(item);

            // after save action
            var args = new DataOperationArgs { DataContext = db, OperationEvent = DataOperationEvents.AfterInsert, Parameters = _serviceParameters };
            form.AfterInsert(item, args);

            return item;
        }

        public object UpdateItem(IDynamicEditForm form, object item)
        {
            var db = GetDbContext(form);
            db.Update(item);
            return item;
        }

        private DataContextBase _db;

        private DataContextBase GetDbContext(IDynamicEditForm form)
        {
            if (_db != null)
            {
                return _db;
            }

            var type = form.GetDbContextType();
            _db = Activator.CreateInstance(type) as DataContextBase;
            return _db;
        }

        public System.Collections.IList GetEntityData(IDynamicEditForm form, Type entity)
        {
            var db = GetDbContext(form);
            var result = db.Get(entity).ToDynamicList<object>();
            return result;
        }
    }
}
