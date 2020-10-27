using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public class Field
    {
        public Type DataType { get; set; }
        public Type ControlType { get; set; }
        public Type ViewModeControlType { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
    }
}
