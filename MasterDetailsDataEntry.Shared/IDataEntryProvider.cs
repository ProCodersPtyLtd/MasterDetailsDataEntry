using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IDataEntryProvider
    {
        IEnumerable<DataField> GetFormFields(IModelDefinitionForm form);
        IEnumerable<DataField> GetFormFields(Type formType);
        Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetFormFields(IMultiModelDefinitionForm form);
        Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetMultiFormFields(Type formType);

        System.Collections.IList GetModelData(IModelDefinitionForm form);
        object GetModelData(IModelDefinitionForm form, int Id);
        Tuple<object, System.Collections.IList> GetModelData(IMultiModelDefinitionForm form, int Id);
    }
}
