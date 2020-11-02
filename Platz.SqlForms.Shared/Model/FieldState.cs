using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class FieldState : Field
    {
        public int RowIndex { get; set; }
        public object Value { get; set; }
        public List<string> ValidationMessages { get; set; } = new List<string>();

        public FieldState()
        { }

        public FieldState(Field source)
        {
            CopyFrom(source);
        }

        public FieldState(Field source, int rowIndex) : this(source)
        {
            RowIndex = rowIndex;
        }

        public void CopyFrom(Field source)
        {
            DataType = source.DataType;
            ControlType = source.ControlType;
            ViewModeControlType = source.ViewModeControlType;
            Label = source.Label;
            Required = source.Required;
            Hidden = source.Hidden;
            Format = source.Format;
        }
    }
}
