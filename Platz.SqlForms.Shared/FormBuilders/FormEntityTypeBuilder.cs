using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Platz.SqlForms
{
    public class FormEntityTypeBuilder
    {
        protected FieldBuilder _fieldBuilder;
        protected Dictionary<string, DataField> _fields;
        protected readonly List<ActionRouteLink> _contextLinks = new List<ActionRouteLink>();
        public FieldBuilder FieldBuilder { get { return _fieldBuilder; } }
        public IEnumerable<DataField> Fields {  get { return _fields.Values; } }
        public IEnumerable<ActionRouteLink> ContextLinks{  get { return _contextLinks; } }
        public List<DialogButtonDetails> DialogButtons { get; private set; }  = new List<DialogButtonDetails>();
        public List<DialogButtonNavigationDetails> DialogButtonNavigations { get; private set; }  = new List<DialogButtonNavigationDetails>();
        public List<EntityDataOperationDetails> DataOperationActions { get; private set; } = new List<EntityDataOperationDetails>();
        public Dictionary<string, DataField> GetFieldDictionary()
        {
            return _fields;
        }
    }

    public class FormEntityTypeBuilder<TEntity> : FormEntityTypeBuilder where TEntity : class
    {
        protected int _propertyOrder;

        public FormEntityTypeBuilder()
        {
            // order of properties in the Entity has a low priority - all mentioned properties will be moved up 
            int order = 1000;
            _propertyOrder = 0;
            // pre-populate all fields
            var fields = typeof(TEntity).GetSimpleTypeProperties().Select(p => new DataField { BindingProperty = p.Name, DataType = p.PropertyType, Order = order++ });
            _fields = fields.ToDictionary(f => f.BindingProperty, f => f);
        }

        public virtual FieldBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            _propertyOrder++;
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            _fields[bindingProperty].Order = _propertyOrder;

            // explicitly mentioned property is not hidden anymore
            _fields[bindingProperty].Hidden = false;
            var result = new FieldBuilder<TProperty, TEntity>(_fields[bindingProperty]);
            _fieldBuilder = result;
            return result;
        }

        public virtual void InlineButton(string buttonText, string hint = null)
        {
            var bindingProperty = buttonText;
            _fields[bindingProperty] = new DataField { Button = true, BindingProperty = bindingProperty, Label = hint };
        }

        public virtual FormEntityTypeBuilder<TEntity> ContextButton(string buttonText, string actionLinkText)
        {
            _contextLinks.Add(new ActionRouteLink { Text = buttonText, LinkText = actionLinkText });
            return this;
        }

        public virtual FormEntityTypeBuilder<TEntity> DialogButton(ButtonActionTypes actionType, string buttonText = null, string hint = null, string actionLinkText = null)
        {
            DialogButtons.Add(new DialogButtonDetails { Action = actionType, Text = buttonText, Hint = hint, LinkText = actionLinkText });
            return this;
        }

        #region CRUD delegates
        public virtual FormEntityTypeBuilder<TEntity> AfterSave(Action<TEntity, DataOperationArgs> method)
        {
            Action<object, DataOperationArgs> action = new Action<object, DataOperationArgs>((f,a) => method((TEntity)f, a));
            var op = new EntityDataOperationDetails { EntityType = typeof(TEntity), Method = action, OperationEvent = DataOperationEvents.AfterInsert };
            DataOperationActions.Add(op);
            return this;
        }
        #endregion

        public virtual FormEntityTypeBuilder<TEntity> DialogButton(string actionLinkText, ButtonActionTypes actionType, string buttonText = null, string hint = null)
        {
            DialogButtons.Add(new DialogButtonDetails { Action = actionType, Text = buttonText, Hint = hint, LinkText = actionLinkText });
            return this;
        }

        public virtual FormEntityTypeBuilder<TEntity> DialogButtonNavigation(string actionLinkText, params ButtonActionTypes[] actions)
        {
            DialogButtonNavigations.Add(new DialogButtonNavigationDetails { LinkText = actionLinkText, Actions = actions.ToList() });
            return this;
        }

        // hide all except mentioned explicitly
        public virtual void ExcludeAll()
        {
            _fields.Values.ToList().ForEach(f => f.Hidden = true);
        }
    }

    //public class Entity<TEntity> : FormEntityTypeBuilder<TEntity> where TEntity : class
    //{ }
}
