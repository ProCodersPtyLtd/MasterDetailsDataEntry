using Platz.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class FormRuleResult
    {
        public string RuleName { get; set; }
        public string Message { get; set; }
        public bool IsFailed { get; set; }

        public FormRuleResult()
        { }

        public FormRuleResult(string ruleName, string message)
        {
            RuleName = ruleName;
            Message = message;
            IsFailed = true;
        }

        public FormRuleResult(string message) : this(StackTraceHelper.GetCallingMethod(), message)
        {
        }
    }
}
