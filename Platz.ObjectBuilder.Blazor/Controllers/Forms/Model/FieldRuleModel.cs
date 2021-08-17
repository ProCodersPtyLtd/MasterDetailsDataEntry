using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Model
{
    public class FieldRuleModel
    {
        public bool IsDirty { get; set; }
        public string Name { get; set; }
        public FormRuleTriggers Trigger { get; set; }
        public string Code { get; set; }

        public string DisplayName
        {
            get { return $"On{Trigger.ToString()} : {Name}"; }
        }
    }
}
