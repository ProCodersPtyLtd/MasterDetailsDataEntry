using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Expressions
{
    public interface IObjectResolver
    {
        string[] GetQueryFields();
        string[] GetQueryParams();
        string[] GetOperators();
        Dictionary<int, string[]> GetPriorityOperators();
        string[] GetCompareOperators();
        char GetStringLiteralSymbol();
    }
}
