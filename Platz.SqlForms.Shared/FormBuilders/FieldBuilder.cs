using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace Platz.SqlForms
{
    public class FieldBuilder2
    {
        protected DataField _field;
        public DataField Field { get { return _field; } }
    }

    public class FieldBuilder2<TProperty, TEntity> : FieldBuilder2
    {

        public FieldBuilder2(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public FieldBuilder2(DataField field)
        {
            _field = field;
        }

        public virtual FieldBuilder2<TProperty, TEntity> Rule(Func<TEntity, FormRuleResult> method)
        {
            
            return this;
        }

        //public virtual FieldBuilder2<TProperty> IsRequired(bool required = true)
        //{
        //    _field.Required = required;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> IsHidden(bool hidden = true)
        //{
        //    _field.Hidden = hidden;
        //    return this;
        //}
        //public virtual FieldBuilder2<TProperty> IsReadOnly(bool readOnly = true)
        //{
        //    _field.ReadOnly = readOnly;
        //    return this;
        //}
        //public virtual FieldBuilder2<TProperty> IsFilter(bool filter = true)
        //{
        //    _field.Filter = filter;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> IsUnique(bool unique = true)
        //{
        //    _field.Unique = unique;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> Label(string label)
        //{
        //    _field.Label = label;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> Format(string format)
        //{
        //    _field.Format = format;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> Control(Type controlType)
        //{
        //    _field.ControlType = controlType;
        //    return this;
        //}

        //public virtual FieldBuilder2<TProperty> ControlReadOnly(Type controlType)
        //{
        //    _field.ViewModeControlType = controlType;
        //    return this;
        //}


    }
}
