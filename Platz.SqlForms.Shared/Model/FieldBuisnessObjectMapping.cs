using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class FieldBuisnessObjectMapping
    {
        public bool IsLoad { get; set; }
        public bool IsLoadList { get; set; }
        public bool IsSave { get; set; }
        public Func<object, object[], object> Load;
        public Func<object, object[], IEnumerable<object>> LoadList;
        public Action<object, object[]> Save;
        public List<BuisnessObjectMapping> SavingMappings { get; set; } = new List<BuisnessObjectMapping>();
    }

    public class BuisnessObjectMapping
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
