using Microsoft.EntityFrameworkCore;
using Platz.SqlForms;
using Platz.SqlForms.Shared;
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
        // protected Dictionary<string, DataField> _fields;

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

        IEnumerable<DataField> IModelDefinitionForm.GetEntityFields(Type entity)
        {
            if (entity == null)
            {
                return ((IModelDefinitionForm)this).GetDetailsFields();
            }

            var entityIndex = _builder.Entities.IndexOf(entity);
            var fields = _builder.Builders[entityIndex].Fields.OrderBy(f => f.Order);

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), entity);
            }

            return fields;
        }

        IEnumerable<DataField> IModelDefinitionForm.GetDetailsFields()
        {
            var fields = new List<DataField>();
            // _builder.Builders.Where(b => b != _builder.Builders[_builder.MasterEntityIndex]).Single().Fields.OrderBy(f => f.Order);
            
            for (int i = 0; i < _builder.Builders.Count; i++)
            {
                if (i != _builder.MasterEntityIndex)
                {
                    fields.AddRange(_builder.Builders[i].Fields.OrderBy(f => f.Order));
                    // we expect only one non master Entity
                    break;
                }
            }
            
            // _fields = fields.ToDictionary(f => f.BindingProperty, f => f);

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), GetDetailsType());
            }

            return fields;
        }

        public Type GetEntityType()
        {
            return _builder.Entities.First();
        }

        public Type GetDetailsType()
        {
            for (int i = 0; i < _builder.Builders.Count; i++)
            {
                if (i != _builder.MasterEntityIndex)
                {
                    return _builder.Entities[i];
                }
            }

            throw new Exception("Cannot find Details Entity");
        }

        // ToDo: why it is implemented explicitly? 
        Type IModelDefinitionForm.GetMasterType()
        {
            return _builder.Entities[_builder.MasterEntityIndex];
        }

        IEnumerable<DataField> IModelDefinitionForm.GetMasterFields()
        {
            var fields = _builder.Builders[_builder.MasterEntityIndex].Fields.OrderBy(f => f.Order);

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), (this as IModelDefinitionForm).GetMasterType());
            }

            return fields;
        }

        // ToDo: should it be moved to DataEntryProvider?
        public string GetFieldFormat(DataField field)
        {
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
