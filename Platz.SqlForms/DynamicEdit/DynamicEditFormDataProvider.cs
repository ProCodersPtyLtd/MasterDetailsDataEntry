using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DynamicEditFormDataProvider : IDynamicEditFormDataProvider
    {
        private object[] _serviceParameters;
        private readonly CustomStoreDynamicEditFormDataProvider _storeProvider;
        private readonly EntityFrameworkDynamicEditFormDataProvider _entityProvider;

        public DynamicEditFormDataProvider()
        {
            // ToDo: use ServiceProvider here to instantiate them
            _storeProvider = new CustomStoreDynamicEditFormDataProvider();
            _entityProvider = new EntityFrameworkDynamicEditFormDataProvider();
        }

        public void SetParameters(object[] serviceParameters)
        {
            _serviceParameters = serviceParameters;
            _storeProvider.SetParameters(_serviceParameters);
            _entityProvider.SetParameters(_serviceParameters);
        }

        public IEnumerable<DataField> GetFormFields(IDynamicEditForm form)
        {
            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                return _entityProvider.GetFormFields(form);
            }

            return _storeProvider.GetFormFields(form);
        }

        // CRUD
        public object GetItem(IDynamicEditForm form, int id)
        {
            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                return _entityProvider.GetItem(form, id);
            }

            return _storeProvider.GetItem(form, id);
        }

        public void DeleteItem(IDynamicEditForm form, object item)
        {
            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                _entityProvider.DeleteItem(form, item);
                return;
            }

            _storeProvider.DeleteItem(form, item);
            return;
        }

        public object InsertItem(IDynamicEditForm form, object item)
        {
            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                return _entityProvider.InsertItem(form, item);
            }

            return _storeProvider.InsertItem(form, item);
        }

        public object UpdateItem(IDynamicEditForm form, object item)
        {
            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                return _entityProvider.UpdateItem(form, item);
            }

            return _storeProvider.UpdateItem(form, item);
        }

        public IList GetEntityData(IDynamicEditForm form, Type entity)
        {
            if (entity.IsEnum)
            {
                var result = Enum.GetValues(entity);
                return result;
            }

            var type = form.GetDbContextType();

            if (type.IsSubclassOf(typeof(DbContext)))
            {
                return _entityProvider.GetEntityData(form, entity);
            }

            return _storeProvider.GetEntityData(form, entity);
        }
    }
}
