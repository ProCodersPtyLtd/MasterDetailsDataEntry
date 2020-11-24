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
        public FieldBuilder FieldBuilder { get { return _fieldBuilder; } }
        public IEnumerable<DataField> Fields {  get { return _fields.Values; } }
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

        public virtual FieldBuilder<TProperty> Property<TProperty>([NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var bindingProperty = propertyExpression.Body.ToString().ReplaceLambdaVar();
            // explicitly mentioned property is not hidden anymore
            _fields[bindingProperty].Hidden = false;
            var result = new FieldBuilder<TProperty>(_fields[bindingProperty]);
            _fieldBuilder = result;
            return result;
        }

        public virtual void Button(string buttonText, string hint = null)
        {
            var bindingProperty = buttonText;
            _fields[bindingProperty] = new DataField { Button = true, BindingProperty = bindingProperty, Label = hint };
        }

        // hide all except mentioned explicitly
        public virtual void ExcludeAll()
        {
            _fields.Values.ToList().ForEach(f => f.Hidden = true);
        }
    }
}
