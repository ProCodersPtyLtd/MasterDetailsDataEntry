using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Blazor
{
    public class ComponentTypeResolver : IComponentTypeResolver
    {
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>() 
        {
            // default controls
            { typeof(DefaultTextEditControl).FullName, typeof(TextEdit) },
            { typeof(DefaultDateEditControl).FullName, typeof(DateEdit) },
            { typeof(DefaultCheckboxControl).FullName, typeof(Checkbox) },
            { typeof(DefaultDropdownControl).FullName, typeof(Dropdown) },
            { typeof(DefaultDropdownReadonlyControl).FullName, typeof(DropdownReadonly) },
            // readonly view
            { typeof(DefaultFormattedViewControl).FullName, typeof(FormattedView) },
            { typeof(DefaultReadonlyCheckboxControl).FullName, typeof(CheckboxReadonly) },

            // extended controls
            { typeof(TextEdit).FullName, typeof(TextEdit) }
        };
        
        public ComponentTypeResolver()
        { }

        public Type GetComponentTypeByName(string name)
        {
            return _types[name];
        }
    }
}
