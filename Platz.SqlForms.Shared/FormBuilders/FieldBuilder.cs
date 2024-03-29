﻿using Platz.SqlForms.Shared;
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

    public class FieldBuilder<TProperty, TEntity> : FieldBuilder where TEntity : class
    {

        public FieldBuilder(string bindingProperty)
        {
            _field = new DataField { BindingProperty = bindingProperty, DataType = typeof(TProperty) };
        }

        public FieldBuilder(DataField field)
        {
            _field = field;
        }

        #region DB Context for Business Objects

        //public virtual FieldBuilder<TProperty, TEntity> Load()
        //{
        //    return this;
        //}

        //public virtual FieldBuilder<TProperty, TEntity> Load(Func<TEntity, TProperty> method)
        //{
        //    return this;
        //}

        //public virtual FieldBuilder<TProperty, TEntity> Load(Func<TEntity, IEnumerable<TProperty>> method)
        //{
        //    return this;
        //}

        //public virtual FieldBuilder<TProperty, TEntity> Save()
        //{
        //    return this;
        //}

        //public virtual FieldBuilder<TProperty, TEntity> Save(Action<TEntity> method)
        //{
        //    return this;
        //}
        //public virtual FieldBuilder<TProperty, TEntity> SaveMapping([NotNullAttribute] Expression<Func<TEntity, object>> modelExpression,
        //    [NotNullAttribute] Expression<Func<TProperty, object>> propertyExpression)
        //{
        //    return this;
        //}

        #endregion


        public virtual FieldBuilder<TProperty, TEntity> Rule([NotNullAttribute] Func<TEntity, FormRuleResult> method, FormRuleTriggers trigger = FormRuleTriggers.ChangeSubmit)
        {
            // https://stackoverflow.com/questions/3444246/convert-actiont-to-actionobject
            Func<object, FormRuleResult> rule = new Func<object, FormRuleResult>(f => method((TEntity)f));
            _field.Rules.Add(new FieldRule { Method = rule, Trigger = trigger });
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Rule([NotNullAttribute] Func<RuleArgs<TEntity>, FormRuleResult> method,
            FormRuleTriggers trigger = FormRuleTriggers.ChangeSubmit)
        {
            // https://stackoverflow.com/questions/3444246/convert-actiont-to-actionobject
            Func<object, FormRuleResult> rule = new Func<object, FormRuleResult>(f => method((RuleArgs<TEntity>)f));
            _field.Rules.Add(new FieldRule { MethodArgs = rule, EntityType = typeof(TEntity), Trigger = trigger });
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsRequired(bool required = true)
        {
            _field.Required = required;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsHidden(bool hidden = true)
        {
            _field.Hidden = hidden;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsPrimaryKey(bool pk = true)
        {
            _field.PrimaryKey = pk;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsReadOnly(bool readOnly = true)
        {
            _field.ReadOnly = readOnly;
            return this;
        }
        public virtual FieldBuilder<TProperty, TEntity> IsFilter(bool filter = true)
        {
            _field.Filter = filter;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> IsUnique(bool unique = true)
        {
            _field.Unique = unique;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Label(string label)
        {
            _field.Label = label;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Filter(FieldFilterType type)
        {
            _field.Filter = true;
            _field.FilterType = type;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Format(string format)
        {
            _field.Format = format;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> Control(Type controlType)
        {
            _field.ControlType = controlType;
            return this;
        }

        public virtual FieldBuilder<TProperty, TEntity> ControlReadOnly(Type controlType)
        {
            _field.ViewModeControlType = controlType;
            return this;
        }

        public virtual DropdownFieldBuilder<TEntity2> Dropdown<TEntity2>()
        {
            return new DropdownFieldBuilder<TEntity2>(this, _field);
        }

        public class DropdownFieldBuilder<TEntity2>
        {
            private FieldBuilder<TProperty, TEntity> _parent;
            private DataField _field;

            public DropdownFieldBuilder(FieldBuilder<TProperty, TEntity> parent, DataField field)
            {
                _field = field;
                _parent = parent;
            }

            public FieldBuilder<TProperty, TEntity> Set<TKey2, TKey3>(
                [NotNullAttribute] Expression<Func<TEntity2, TKey2>> id,
                [NotNullAttribute] Expression<Func<TEntity2, TKey3>> name)
            {
                _field.ControlType = typeof(DefaultDropdownControl);
                _field.ViewModeControlType = typeof(DefaultDropdownReadonlyControl);
                
                _field.SelectEntityType = typeof(TEntity2);
                _field.SelectIdProperty = id.Body.ToString().ReplaceLambdaVar();
                _field.SelectNameProperty = name.Body.ToString().ReplaceLambdaVar();

                return _parent;
            }
        }
    }
}
