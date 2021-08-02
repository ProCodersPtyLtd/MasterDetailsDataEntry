using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class Field
    {
        public Type DataType { get; set; }
        public Type ControlType { get; set; }
        public Type ViewModeControlType { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        public string Format { get; set; }
        public FieldFilterType FilterType { get; set; }
        public List<FieldRule> Rules { get; set; } = new List<FieldRule>();
        public FieldBuisnessObjectMapping BuisnessObjectMapping { get; set; } = new FieldBuisnessObjectMapping();
    }

    
}
