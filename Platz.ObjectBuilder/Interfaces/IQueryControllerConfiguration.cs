using Platz.ObjectBuilder.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IQueryControllerConfiguration
    {
        IStoreSchemaReader Reader { get; }
        IStoreSchemaStorage Storage { get; }
        IStoreSchemaReaderParameters ReaderParameters { get; }
        IObjectResolver Resolver { get; }
        SqlExpressionEngine ExpressionEngine { get; }
    }
}
