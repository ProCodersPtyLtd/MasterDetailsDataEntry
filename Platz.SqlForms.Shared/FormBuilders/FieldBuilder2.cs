using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace Platz.SqlForms
{
    public class FieldBuilder
    {
        protected DataField _field;
        public DataField Field { get { return _field; } }
    }

    public class FieldBuilder<TProperty> : FieldBuilder
    {

        public FieldBuilder(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public FieldBuilder(DataField field)
        {
            _field = field;
        }

        public virtual FieldBuilder<TProperty> Rule(Func<TProperty, FormRuleResult> method)
        {
            
            return this;
        }

        public virtual FieldBuilder<TProperty> IsRequired(bool required = true)
        {
            _field.Required = required;
            return this;
        }

        public virtual FieldBuilder<TProperty> IsHidden(bool hidden = true)
        {
            _field.Hidden = hidden;
            return this;
        }
        public virtual FieldBuilder<TProperty> IsReadOnly(bool readOnly = true)
        {
            _field.ReadOnly = readOnly;
            return this;
        }
        public virtual FieldBuilder<TProperty> IsFilter(bool filter = true)
        {
            _field.Filter = filter;
            return this;
        }

        public virtual FieldBuilder<TProperty> IsUnique(bool unique = true)
        {
            _field.Unique = unique;
            return this;
        }

        public virtual FieldBuilder<TProperty> Label(string label)
        {
            _field.Label = label;
            return this;
        }

        public virtual FieldBuilder<TProperty> Format(string format)
        {
            _field.Format = format;
            return this;
        }

        public virtual FieldBuilder<TProperty> Control(Type controlType)
        {
            _field.ControlType = controlType;
            return this;
        }

        public virtual FieldBuilder<TProperty> ControlReadOnly(Type controlType)
        {
            _field.ViewModeControlType = controlType;
            return this;
        }

        public virtual DropdownFieldBuilder<TEntity> Dropdown<TEntity>()
        {
            return new DropdownFieldBuilder<TEntity>(this, _field);
        }

        public class DropdownFieldBuilder<TEntity>
        {
            private FieldBuilder<TProperty> _parent;
            private DataField _field;

            public DropdownFieldBuilder(FieldBuilder<TProperty> parent, DataField field)
            {
                _field = field;
                _parent = parent;
            }

            public FieldBuilder<TProperty> Set<TKey2, TKey3>(
                [NotNullAttribute] Expression<Func<TEntity, TKey2>> id,
                [NotNullAttribute] Expression<Func<TEntity, TKey3>> name)
            {
                _field.ControlType = typeof(DefaultDropdownControl);
                _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);
                
                _field.SelectEntityType = typeof(TEntity);
                _field.SelectIdProperty = id.Body.ToString().ReplaceLambdaVar();
                _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

                return _parent;
            }
        }
    }
}
