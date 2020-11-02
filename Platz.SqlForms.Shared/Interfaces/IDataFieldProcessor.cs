using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Shared
{
    public interface IDataFieldProcessor
    {
        List<DataField> PrepareFields(List<DataField> list, Type type);
        void ResolveLabel(Type type, DataField field);
        void ResolveControlType(Type type, DataField field);
    }
}
