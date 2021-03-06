﻿using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public abstract class DataServiceBase : IDataForm
    {
        protected readonly IDataFieldProcessor _dataFieldProcessor;
        protected DataServiceFormBuilder _builder;

        private Dictionary<Type, string> _formats = new Dictionary<Type, string>()
        {
            { typeof(DateTime), "dd/MM/yyyy" }
            ,{ typeof(DateTime?), "dd/MM/yyyy" }
        };

        public DataServiceBase()
        {
            _dataFieldProcessor = new DefaultDataFieldProcessor();
            _builder = new DataServiceFormBuilder();
            Define(_builder);
        }

        protected virtual void Define(DataServiceFormBuilder builder)
        {
        }

        public IList ExecuteListQuery(object[] parameters)
        {
            var result = _builder.ListQuery(parameters) as IList;
            return result;
        }

        public IList ExecuteListQuery(QueryOptions options, object[] parameters)
        {
            IList result;

            if (_builder.ListQueryOptions != null)
            {
                result = _builder.ListQueryOptions(options, parameters) as IList;
            }
            else
            {
                result = _builder.ListQuery(parameters) as IList;
            }

            return result;
        }

        public IEnumerable<ActionRouteLink> GetContextLinks()
        {
            var result = _builder.Builders.SelectMany(b => b.ContextLinks);
            return result;
        }

        public IEnumerable<DataField> GetDetailsFields()
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

        public Type GetEntityType()
        {
            return _builder.Entities.First();
        }
        public string GetFieldFormat(DataField field)
        {
            var format = field.Format ?? FindDefaultFormat(field.DataType);
            return format;
        }

        public abstract Type GetDbContextType();

        private string FindDefaultFormat(Type dataType)
        {
            if (_formats.ContainsKey(dataType))
            {
                return _formats[dataType];
            }

            return "";
        }

    }

    public abstract class DataServiceBase<T> : DataServiceBase where T: DbContext
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

    public abstract class StoreDataServiceBase<T> : DataServiceBase where T : DataContextBase
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
