using Microsoft.EntityFrameworkCore;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers
{
    public class EntityFrameworkQueryControllerParameters : IQueryControllerParameters
    {
        public Type DbContextType { get; set; }
    }

    public class EntityFrameworkQueryController : QueryControllerBase
    {
        private EntityFrameworkQueryControllerParameters _parameters;

        public EntityFrameworkQueryController()
        {
            _storage = new FileStoreSchemaStorage();
        }

        public override void SetParameters(IQueryControllerParameters parameters)
        {
            _resolver = new SqlJsonObjectResolver();
            _expressions = new SqlExpressionEngine(_resolver);

            _parameters = parameters as EntityFrameworkQueryControllerParameters;

            if (_parameters == null)
            {
                throw new Exception($"EntityFrameworkQueryController expects EntityFrameworkQueryControllerParameters.");
            }

            _reader = new EntityFrameworkStoreSchemaReader();
            var readerParameters = new EntityFrameworkStoreSchemaReaderParameters();
            readerParameters.DbContextType = _parameters.DbContextType;
            _readerParameters = readerParameters;
        }
    }
}
