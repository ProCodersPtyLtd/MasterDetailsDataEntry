using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IMultiModelDefinitionForm : IModelDefinitionForm
    {
        public Tuple<List<DataField>, List<DataField>> GetFields();
    }
}
