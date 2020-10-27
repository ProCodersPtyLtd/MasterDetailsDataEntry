using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public class DataField : Field
    {
        public string BindingProperty { get; set; }
        public bool PrimaryKey { get; set; }
        public bool ForeignKey { get; set; }
        public string FilterProperty { get; set; }

        public DataField()
        { }

        public DataField(Field source)
        {
            CopyFrom(source);
        }

        public void CopyFrom(Field source)
        {
            DataType = source.DataType;
            ControlType = source.ControlType;
            ViewModeControlType = source.ViewModeControlType;
            Label = source.Label;
            Required = source.Required;
            Hidden = source.Hidden;
        }
    }
}
