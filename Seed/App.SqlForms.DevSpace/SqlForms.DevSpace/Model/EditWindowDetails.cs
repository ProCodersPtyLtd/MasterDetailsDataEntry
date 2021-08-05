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
        Unknown = 0,
        ProjectSettings = 10,
        Schema = 20,
        Query = 30,
        Form = 40
    }
}
