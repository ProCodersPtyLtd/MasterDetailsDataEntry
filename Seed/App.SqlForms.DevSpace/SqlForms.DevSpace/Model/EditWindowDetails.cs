using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Model
{
    public class EditWindowDetails
    {
        public EditWindowType Type { get; set; }

        public IStoreObject StoreObject { get; set; }
    }

    public enum EditWindowType
    {
        ProjectSettings = 1,
        Schema = 10,
        Query = 20,
        Form = 30
    }
}
