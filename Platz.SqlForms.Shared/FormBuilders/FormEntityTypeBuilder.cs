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
    }

    public class FormEntityTypeBuilder<TEntity> : FormEntityTypeBuilder where TEntity : class
    {
        public FormEntityTypeBuilder()
        {
            int order = 0;
            // pre-populate all fields
            var fields = typeof(TEntity).GetSimpleTypeProperties().Select(p => new DataField { BindingProperty = p.Name, DataType = p.PropertyType, Order = order++ });
            _fields = fields.ToDictionary(f => f.BindingProperty, f => f);
        }

        public virtual FieldBuilder<TProperty, TEntity> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            // explicitly mentioned property is not hidden anymore
            _fields[bindingProperty].Hidden = false;
            var result = new FieldBuilder<TProperty, TEntity>(_fields[bindingProperty]);
            _fieldBuilder = result;
            return result;
        }

        public virtual void Button(string buttonText, string hint = null)
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
}
