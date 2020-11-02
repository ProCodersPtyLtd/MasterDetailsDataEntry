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
            var result = new FieldBuilder<TProperty>(_fields[bindingProperty]);
            _fieldBuilder = result;
            return result;
        }

        
    }
}
