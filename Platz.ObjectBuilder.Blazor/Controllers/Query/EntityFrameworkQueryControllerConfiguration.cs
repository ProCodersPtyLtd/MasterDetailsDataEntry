using Microsoft.EntityFrameworkCore;
using Platz.ObjectBuilder.Blazor.Controllers.Logic;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder
{
    public class EntityFrameworkQueryControllerConfiguration : IQueryControllerConfiguration
    {
        public IStoreSchemaReader Reader { get; private set; }

        public IStoreSchemaStorage Storage { get; private set; }

        public IStoreSchemaReaderParameters ReaderParameters { get; private set; }

        public IObjectResolver Resolver { get; private set; }

        public SqlExpressionEngine ExpressionEngine { get; private set; }

        public EntityFrameworkQueryControllerConfiguration(QueryControllerParameters parameters)
        {
            Storage = new FileStoreSchemaStorage();
            Resolver = new SqlJsonObjectResolver();
            ExpressionEngine = new SqlExpressionEngine(Resolver);
            Reader = new EntityFrameworkStoreSchemaReader();
            ReaderParameters = new EntityFrameworkStoreSchemaReaderParameters { DbContextType = parameters.DbContextType };
        }
    }

    //public class EntityFrameworkQueryController : QueryControllerBase
    //{
    //    private QueryControllerParameters _parameters;

    //    public EntityFrameworkQueryController(IQueryBuilderEngine engine): base(engine)
    //    {
    //        _storage = new FileStoreSchemaStorage();
    //    }

    //    public override void SetParameters(QueryControllerParameters parameters)
    //    {
    //        _resolver = new SqlJsonObjectResolver();
    //        _expressions = new SqlExpressionEngine(_resolver);

    //        _parameters = parameters;

    //        if (_parameters == null)
    //        {
    //            throw new Exception($"EntityFrameworkQueryController expects EntityFrameworkQueryControllerParameters.");
    //        }

    //        _reader = new EntityFrameworkStoreSchemaReader();
    //        var readerParameters = new EntityFrameworkStoreSchemaReaderParameters();
    //        readerParameters.DbContextType = _parameters.DbContextType;
    //        _readerParameters = readerParameters;
    //    }
    //}
}
