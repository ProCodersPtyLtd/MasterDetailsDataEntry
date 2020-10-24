using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IMultiModelDefinitionForm : IModelDefinitionForm
    {
        public Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetFields();
    }
}
