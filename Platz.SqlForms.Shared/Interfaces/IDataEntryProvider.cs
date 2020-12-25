using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDataEntryProvider
    {
        // Fields
        IEnumerable<DataField> GetFormFields(IModelDefinitionForm form, Type entity = null);
        System.Collections.IList GetFilteredModelData(IModelDefinitionForm form, int? filterValue, Type entity = null);


        // CRUD
        object InsertItem(IModelDefinitionForm form, object item);
        object UpdateItem(IModelDefinitionForm form, object item);
        void DeleteItem(IModelDefinitionForm form, object item);

        // Validation
        bool IsPropertyValueNotUnique(IModelDefinitionForm form, object item, string bindingProperty, Type entity);

        // ToDo: it looks like the second specialization - it should be moved to another interface
        System.Collections.IList GetEntityData(IDataForm form, Type entity);

    }
}
