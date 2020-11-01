using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public class DataField : Field
    {
        public string BindingProperty { get; set; }
        public bool PrimaryKey { get; set; }
        public PrimaryKeyGeneratedTypes PrimaryKeyGeneratedType { get; set; }
        public bool Filter { get; set; }

        [Obsolete]
        public string FilterProperty { get; set; }
        [Obsolete]
        public bool ForeignKey { get; set; }

        // Select
        public Type SelectEntityType { get; set; }
        public string SelectIdProperty { get; set; }
        public string SelectNameProperty { get; set; }

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
            Format = source.Format;
        }
    }
}
