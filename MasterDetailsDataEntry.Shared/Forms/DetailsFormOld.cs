using MasterDetailsDataEntry.Shared.Controls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MasterDetailsDataEntry.Shared.Forms
{
    public abstract class DetailsFormOld<D> : IModelDefinitionForm
        where D : class
    {
        private readonly IDataFieldProcessor _dataFieldProcessor;
        private Dictionary<string, DataField> _fields;
        private Type _contextType;

        private Dictionary<Type, string> _formats =  new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        protected abstract void Define();

        public DetailsFormOld()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _fields = new Dictionary<string, DataField>();
            Define();
        }

        public string GetFieldFormat(string bindingProperty)
        {
            var field = _fields[bindingProperty];
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }

        // defenition
        public DetailsFormOld<D> UseDefaultFormat<TContext>(Type dataType, string format)
            where TContext : DbContext
        {
            _formats[dataType] = format;
            return this;
        }

        public DetailsFormOld<D> Use<TContext>()
            where TContext : DbContext
        {
            _contextType = typeof(TContext);
            return this;
        }

        public DetailsFormOld<D> PrimaryKey<TKey>(Expression<Func<D, TKey>> selector)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
            var field = FindField(bindingProperty);
            field.BindingProperty = bindingProperty;
            field.PrimaryKey = true;
            return this;
        }

        public DetailsFormOld<D> ForeignKey<TKey>(Expression<Func<D, TKey>> selector)
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

        public DetailsFormOld<D> FilterBy<TKey>(Expression<Func<D, TKey>> selector)
        {
            var filterProperty = selector.Body.ToString().ReplaceLambdaVar();
            var field = FindField(filterProperty);
            field.FilterProperty = filterProperty;
            return this;
        }

        public DetailsFormOld<D> Field<TKey>(Expression<Func<D, TKey>> selector, Field field)
        {
            var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
            var f = FindField(bindingProperty);
            f.CopyFrom(field);
            f.BindingProperty = bindingProperty;
            return this;
        }

        //public DetailsForm<D> DropdwonField<TEntity, TKey, TKey2, TKey3>(Expression<Func<D, TKey>> selectedId,
        //    //Expression<Func<D, IEnumerable<TKey>>> items,
        //    Expression<Func<TEntity, TKey2>> id,
        //    Expression<Func<TEntity, TKey3>> name)
        //{
        //    return this;
        //}

        public class DropdownField<TEntity>
        {
            private DetailsFormOld<D> _parent;

            public DropdownField(DetailsFormOld<D> parent)
            {
                _parent = parent;
            }

            public DetailsFormOld<D> Field<TKey, TKey2, TKey3>(Expression<Func<D, TKey>> selector, 
                Expression<Func<TEntity, TKey2>> id, 
                Expression<Func<TEntity, TKey3>> name,
                Field field = null)
            {
                var bindingProperty = selector.Body.ToString().ReplaceLambdaVar();
                var f = _parent.FindField(bindingProperty);

                if (field != null)
                {
                    f.CopyFrom(field);
                }

                f.BindingProperty = bindingProperty;
                f.ControlType = typeof(DefaultDropdownControl);
                f.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);
                f.SelectEntityType = typeof(TEntity);
                f.SelectIdProperty = id.Body.ToString().ReplaceLambdaVar();
                f.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

                return _parent;
            }
        }

        public DropdownField<TEntity> Dropdown<TEntity>()
        {
            return new DropdownField<TEntity>(this);
        }

        // Use Include or Exclude approach, don't use both
        public DetailsFormOld<D> IncludeField<TKey>(Expression<Func<D, TKey>> selector)
        {
            throw new NotImplementedException();
            return this;
        }

        public DetailsFormOld<D> ExcludeField<TKey>(Expression<Func<D, TKey>> selector)
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

        IEnumerable<DataField> IModelDefinitionForm.GetDetailsFields()
        {
            var fields = _dataFieldProcessor.PrepareFields(_fields.Values.ToList(), typeof(D));
            return fields;
        }
    }
}
