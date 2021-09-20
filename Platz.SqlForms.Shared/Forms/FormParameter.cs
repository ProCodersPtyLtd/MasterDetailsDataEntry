using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class FormParameter
    {
        public string Name {  get; set; }
        public Type Type { get; set; }
        public object Value {  get; set; }

        public FormParameter(string name, object value, Type type = null)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}
