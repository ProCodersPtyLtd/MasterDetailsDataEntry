using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class PropertyContextBuilder
    {
        protected DataField _field;
        public DataField Field { get { return _field; } }
    }

    public class PropertyContextBuilder<TProperty, TEntity> : PropertyContextBuilder where TEntity : class
    {

        public PropertyContextBuilder(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public PropertyContextBuilder(DataField field)
        {
            _field = field;
        }

        public virtual PropertyContextBuilder<TProperty, TEntity> Load()
        {
            _field.BuisnessObjectMapping.IsLoad = true;
            return this;
        }

        public virtual PropertyContextBuilder<TProperty, TEntity> Load(Func<TEntity, object[], TProperty> method)
        {
            _field.BuisnessObjectMapping.IsLoad = true;
            Func<object, object[], object> load = new Func<object, object[], object>((f, o) => method((TEntity)f, o));
            _field.BuisnessObjectMapping.Load = load;
            return this;
        }

        public virtual PropertyContextBuilder<TProperty, TEntity> Load(Func<TEntity, object[], IEnumerable<TProperty>> method)
        {
            _field.BuisnessObjectMapping.IsLoadList = true;
            Func<object, object[], IEnumerable<object>> loadList = new Func<object, object[], IEnumerable<object>>((f, o) => method((TEntity)f, o).Cast<object>().ToList());
            _field.BuisnessObjectMapping.LoadList = loadList;
            return this;
        }

        public virtual PropertyContextBuilder<TProperty, TEntity> Save()
        {
            _field.BuisnessObjectMapping.IsSave = true;
            return this;
        }

        public virtual PropertyContextBuilder<TProperty, TEntity> Save(Action<TEntity, object[]> method)
        {
            _field.BuisnessObjectMapping.IsSave = true;
            Action<object, object[]> save = new Action<object, object[]>((f, o) => method((TEntity)f, o));
            _field.BuisnessObjectMapping.Save = save;
            return this;
        }
        public virtual PropertyContextBuilder<TProperty, TEntity> SavingMapping([NotNullAttribute] Expression<Func<TEntity, object>> modelExpression,
            [NotNullAttribute] Expression<Func<TProperty, object>> propertyExpression)
        {
            _field.BuisnessObjectMapping.IsSave = true;
            var m = new BuisnessObjectMapping { From = modelExpression.Body.ToString().ReplaceLambdaVar(), To = propertyExpression.Body.ToString().ReplaceLambdaVar() };
            _field.BuisnessObjectMapping.SavingMappings.Add(m);
            return this;
        }
    }
}
