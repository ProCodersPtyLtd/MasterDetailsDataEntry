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
    public class JsonSchemaQueryControllerConfiguration : IQueryControllerConfiguration
    {
        public IStoreSchemaReader Reader { get; private set; }

        public IStoreSchemaStorage Storage { get; private set; }

        public IStoreSchemaReaderParameters ReaderParameters { get; private set; }

        public IObjectResolver Resolver { get; private set; }

        public SqlExpressionEngine ExpressionEngine { get; private set; }

        public JsonSchemaQueryControllerConfiguration(QueryControllerParameters parameters)
        {
            Storage = new FileStoreSchemaStorage();
            Resolver = new SqlJsonObjectResolver();
            ExpressionEngine = new SqlExpressionEngine(Resolver);
            Reader = new JsonStoreSchemaReader();
            ReaderParameters = new JsonStoreSchemaReaderParameters { SchemaFile = parameters.SchemaFilename };
        }
    }
}
