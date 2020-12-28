using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using Platz.SqlForms.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DynamicEditFormBase : IDynamicEditForm
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected DynamicFormBuilder _builder;

        private Dictionary<Type, string> _formats = new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        public DynamicEditFormBase()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new DynamicFormBuilder();
            Define(_builder);
        }

        protected virtual void Define(DynamicFormBuilder builder)
        {
        }

        public IEnumerable<DialogButtonDetails> GetButtons()
        {
            var result = _builder.Builders.SelectMany(b => b.DialogButtons);
            return result;
        }

        public IEnumerable<DialogButtonNavigationDetails> GetButtonNavigations()
        {
            var result = _builder.Builders.SelectMany(b => b.DialogButtonNavigations);
            return result;
        }

        public IEnumerable<EntityDataOperationDetails> GetDataOperationActions()
        {
            var result = _builder.Builders.SelectMany(b => b.DataOperationActions);
            return result;
        }

        IEnumerable<DataField> IDynamicEditForm.GetFields()
        {
            var fields = new List<DataField>();

            for (int i = 0; i < _builder.Builders.Count; i++)
            {
                if (i != _builder.MasterEntityIndex)
                {
                    fields.AddRange(_builder.Builders[i].Fields.OrderBy(f => f.Order));
                    // we expect only one non master Entity
                    break;
                }
            }

            if (fields.Any())
            {
                _dataFieldProcessor.PrepareFields(fields.ToList(), GetEntityType());

                // ToDo: we need a way to always specify PK in query builder
                if (!fields.Any(f => f.PrimaryKey))
                {
                    var pk = fields.FirstOrDefault(f => f.BindingProperty.ToLower().EndsWith("id"));
                    pk = pk ?? fields.FirstOrDefault(f => f.BindingProperty.ToLower().StartsWith("id"));
                    pk = pk ?? fields.FirstOrDefault(f => f.BindingProperty.ToLower().Contains("id"));

                    if (pk != null)
                    {
                        pk.PrimaryKey = true;
                    }
                }
            }

            return fields;
        }

        public Type GetEntityType()
        {
            return _builder.Entities.First();
        }

        public abstract Type GetDbContextType();

        public string GetFieldFormat(DataField field)
        {
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }

        public void AfterInsert(object model, DataOperationArgs args)
        {
            var action = GetDataOperationActions().FirstOrDefault(a => a.OperationEvent == DataOperationEvents.AfterInsert);

            if (action != null)
            {
                action.Method(model, args);
            }
        }
    }

    public abstract class DynamicEditFormBase<T> : DynamicEditFormBase where T : DbContext
    {
        public override Type GetDbContextType()
        {
            return typeof(T);
        }

        protected T GetDbContext()
        {
            var context = Activator.CreateInstance<T>();
            return context;
        }
    }
}
