using MasterDetailsDataEntry.Shared;
using Microsoft.EntityFrameworkCore;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DetailsForm<TContext> : IModelDefinitionForm
        where TContext : DbContext
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected FormBuilder _builder;
        protected Dictionary<string, DataField> _fields;

        private Dictionary<Type, string> _formats = new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        public DetailsForm()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new FormBuilder();
            Define(_builder);
        }

        public Type GetDbContextType()
        {
            return typeof(TContext);
        }

        IEnumerable<DataField> IModelDefinitionForm.GetDetailsFields()
        {
            var fields = _builder.Builders.SelectMany(b => b.Fields);
            _fields = fields.ToDictionary(f => f.BindingProperty, f => f);

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), GetDetailsType());
            }

            return fields;
        }

        public Type GetDetailsType()
        {
            return _builder.Entities.Single();
        }

        public string GetFieldFormat(string bindingProperty)
        {
            var field = _fields[bindingProperty];
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        protected abstract void Define(FormBuilder builder);

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }
    }
}
