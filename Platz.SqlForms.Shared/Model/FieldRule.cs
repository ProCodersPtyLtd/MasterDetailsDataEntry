using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class FieldRule
    {
        public FormRuleTriggers Trigger { get; set; }
        public Func<object, FormRuleResult> Method { get; set; }
        public Func<object, object, FormRuleResult> MethodB { get; set; }
        public Type BuilderType { get; set; }
        public FormEntityTypeBuilder EntityBuilder { get; set; }
    }
}
