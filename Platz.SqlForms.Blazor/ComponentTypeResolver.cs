using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Blazor
{
    public class ComponentTypeResolver : IComponentTypeResolver
    {
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>() 
        {
            // default controls
            { typeof(DefaultTextEditControl).FullName, typeof(TextEditComponent) },
            { typeof(DefaultNumberEditControl).FullName, typeof(NumberEditComponent) },
            { typeof(DefaultDateEditControl).FullName, typeof(DateEditComponent) },
            { typeof(DefaultCheckboxControl).FullName, typeof(CheckboxComponent) },
            { typeof(DefaultDropdownControl).FullName, typeof(DropdownComponent) },
            { typeof(DefaultDropdownReadonlyControl).FullName, typeof(DropdownReadonlyComponent) },
            // readonly view
            { typeof(DefaultFormattedViewControl).FullName, typeof(FormattedViewComponent) },
            { typeof(DefaultReadonlyCheckboxControl).FullName, typeof(CheckboxReadonlyComponent) },

            // extended controls
            { typeof(TextEditComponent).FullName, typeof(TextEditComponent) }
        };
        
        public ComponentTypeResolver()
        { }

        public Type GetComponentTypeByName(string name)
        {
            return _types[name];
        }
    }
}
