using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MasterDetailsDataEntry.Shared.Forms
{
    public abstract class DetailsForm<D> : IModelDefinitionForm
        where D : class
    {
        private readonly IDataFieldProcessor _dataFieldProcessor;
        private Dictionary<string, DataField> _fields;
        private Type _contextType;

        protected abstract void Define();

        public DetailsForm()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _fields = new Dictionary<string, DataField>();
            Define();
        }

        public DetailsForm<D> Use<TContext>()
            where TContext : DbContext
        {
            _contextType = typeof(TContext);
            return this;
        }

        public DetailsForm<D> PrimaryKey<TKey>(Expression<Func<D, TKey>> selector)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
            var field = FindField(bindingProperty);
            field.BindingProperty = bindingProperty;
            field.PrimaryKey = true;
            return this;
        }

        public DetailsForm<D> ForeignKey<TKey>(Expression<Func<D, TKey>> selector)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
            var field = FindField(bindingProperty);
            field.BindingProperty = bindingProperty;
            field.ForeignKey = true;
            return this;
        }

        private DataField FindField(string bindingProperty)
        {
            if (!_fields.ContainsKey(bindingProperty))
            {
                _fields[bindingProperty] = new DataField();
            }

            return _fields[bindingProperty];
        }

        public DetailsForm<D> FilterBy<TKey>(Expression<Func<D, TKey>> selector)
        {
            var filterProperty = selector.Body.ToString().ReplaceLambdaVar();
            var field = FindField(filterProperty);
            field.FilterProperty = filterProperty;
            return this;
        }

        public DetailsForm<D> Field<TKey>(Expression<Func<D, TKey>> selector, Field field)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
            var f = FindField(bindingProperty);
            f.CopyFrom(field);
            f.BindingProperty = bindingProperty;
            return this;
        }

        // Use Include or Exclude approach, don't use both
        public DetailsForm<D> IncludeField<TKey>(Expression<Func<D, TKey>> selector)
        {
            throw new NotImplementedException();
            return this;
        }

        public DetailsForm<D> ExcludeField<TKey>(Expression<Func<D, TKey>> selector)
        {
            throw new NotImplementedException();
            return this;
        }

        public IEnumerable<DataField> GetFields()
        {
            return _dataFieldProcessor.PrepareFields(_fields.Values.ToList(), typeof(D));
        }

        public Type GetDbContextType()
        {
            return _contextType;
        }

        public Type GetDetailsType()
        {
            return typeof(D);
        }

        public IEnumerable<DataField> GetDetailsFields()
        {
            var fields = _dataFieldProcessor.PrepareFields(_fields.Values.ToList(), typeof(D));
            return fields;
        }
    }
}
