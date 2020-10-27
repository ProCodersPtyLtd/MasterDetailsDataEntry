using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IComponentTypeResolver
    {
        Type GetComponentTypeByName(string name);
    }
}
