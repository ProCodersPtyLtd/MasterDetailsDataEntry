using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public class FieldState : Field
    {
        public int RowIndex { get; set; }
        public object Value { get; set; }

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
            Label = source.Label;
            Required = source.Required;
            Hidden = source.Hidden;
        }
    }
}
