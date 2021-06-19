using Platz.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class RuleValidationResult
    {
        public string RuleName { get; set; }
        public string Message { get; set; }
        public bool IsFailed { get; set; }
        public List<RuleValidationResult> Results { get; set; }

        public RuleValidationResult()
        { }

        public RuleValidationResult(string ruleName, string message)
        {
            RuleName = ruleName;
            Message = message;
            IsFailed = true;
        }

        public RuleValidationResult(string message) : this(StackTraceHelper.GetCallingClass().Name, message)
        {
        }


    }

}
