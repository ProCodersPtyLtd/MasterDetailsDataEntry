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
            { typeof(DefaultTextEditControl).FullName, typeof(TextEditControl) },
            { typeof(DefaultNumberEditControl).FullName, typeof(NumberEditControl) },
            { typeof(DefaultDateEditControl).FullName, typeof(DateEditControl) },
            { typeof(DefaultCheckboxControl).FullName, typeof(CheckboxControl) },
            { typeof(DefaultDropdownControl).FullName, typeof(DropdownControl) },
            { typeof(DefaultDropdownReadonlyControl).FullName, typeof(DropdownReadonlyControl) },
            // readonly view
            { typeof(DefaultFormattedViewControl).FullName, typeof(FormattedViewControl) },
            { typeof(DefaultReadonlyCheckboxControl).FullName, typeof(CheckboxReadonlyControl) },

            // extended controls
            { typeof(TextEditControl).FullName, typeof(TextEditControl) }
        };
        
        public ComponentTypeResolver()
        { }

        public Type GetComponentTypeByName(string name)
        {
            return _types[name];
        }
    }
}
