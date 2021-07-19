using Microsoft.EntityFrameworkCore;
using Platz.SqlForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static IQueryable<Q> Prepare<Q>(IQueryable<Q> query, QueryOptions options, params object[] parameters)
        {
            query = OrderBy(query, options.SortColumn, options.SortDirection);
            query = ApplyFilters(query, options.Filters);
            return query;
        }

        public static List<Q> GetResult<Q>(IQueryable<Q> query, QueryOptions options, params object[] parameters)
        {
            if (options.ApplyPagination)
            {
                if (options.PageReturnTotalCount == -1)
                {
                    options.PageReturnTotalCount = query.Count();
                }

                var page = query
                    .Skip(options.Page * options.PageSize)
                    .Take(options.PageSize);
                //.GroupBy(e => new { Total = query.Count() })
                //.FirstOrDefault();

                //options.PageReturnTotalCount = page.Key.Total;
                //return page.Select(e => e).ToList();

                return page.ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public static IQueryable<Q> ApplyFilters<Q>(IQueryable<Q> query, List<FieldFilter> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.FilterType)
                {
                    case FieldFilterType.Text:
                        query = WhereLike(query, filter.BindingProperty, filter.Filter);
                        break;
                    case FieldFilterType.TextStarts:
                        query = WhereLike(query, filter.BindingProperty, $"{filter.Filter}%");
                        break;
                    case FieldFilterType.TextEnds:
                        query = WhereLike(query, filter.BindingProperty, $"%{filter.Filter}");
                        break;
                    case FieldFilterType.TextContains:
                        query = WhereLike(query, filter.BindingProperty, $"%{filter.Filter}%");
                        break;
                }
            }

            return query;
        }

        // query = OrderBy(query, options.SortColumn, options.SortDirection);
        public static IQueryable<Q> OrderBy<Q>(IQueryable<Q> query, string colName, SortDirection direction)
        {
            if (!string.IsNullOrWhiteSpace(colName) && direction != SortDirection.None)
            {
                string directionMethod = direction == SortDirection.Asc ? "OrderBy" : "OrderByDescending";
                var parameter = Expression.Parameter(typeof(Q), "x");
                Expression property = Expression.Property(parameter, colName);
                var lambda = Expression.Lambda(property, parameter);

                // REFLECTION: source.OrderBy(x => x.Property)
                var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == directionMethod && x.GetParameters().Length == 2);
                var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(Q), property.Type);
                var result = orderByGeneric.Invoke(null, new object[] { query, lambda });
                return (IQueryable<Q>)result;
            }

            return query;
        }

        public static IQueryable<Q> WhereLike<Q>(IQueryable<Q> query, string colName, string searchPattern)
        {
            var parameter = Expression.Parameter(typeof(Q), "x");
            Expression property = Expression.Property(parameter, colName);
            var lambda = Expression.Lambda(property, parameter);

            //query = query.Where(x => EF.Functions.Like())

            // REFLECTION: source.OrderBy(x => x.Property)
            var functions = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions)));
            var likeFunction = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new Type[] { functions.Type, typeof(string), typeof(string) });

            Expression selectorExpression = Expression.Property(parameter, colName);
            selectorExpression = Expression.Call(null, likeFunction, functions, selectorExpression, Expression.Constant(searchPattern));
            query = query.Where(Expression.Lambda<Func<Q, bool>>(selectorExpression, parameter));


            //var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == directionMethod && x.GetParameters().Length == 2);
            //var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(Q), property.Type);
            //var result = orderByGeneric.Invoke(null, new object[] { query, lambda });
            //return (IQueryable<Q>)result;

            return query;
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
