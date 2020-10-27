using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IMultiModelDefinitionForm : IModelDefinitionForm
    {
        Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetFields();
        Type GetMasterType();
    }
}
