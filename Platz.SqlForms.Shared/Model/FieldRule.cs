using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class FieldRule
    {
        public FormRuleTriggers Trigger { get; set; }
        public Func<object, FormRuleResult> Rule { get; set; }
    }
}
