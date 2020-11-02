using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IComponentTypeResolver
    {
        Type GetComponentTypeByName(string name);
    }
}
