using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public abstract class SubQueryBuilderRuleBase : IQueryBuilderRule
    {
        public abstract RuleValidationResult Validate(IQueryModel qm);

        public RuleValidationResult Validate(IQueryControllerModel model)
        {
            var list = new List<RuleValidationResult>();

            foreach (var q in model.SubQueryList)
            {
                var r = Validate(q);

                if (r != null)
                {
                    list.Add(r);
                }
            }

            if (list.Any())
            {
                var result = new RuleValidationResult($"{this.GetType().Name} rule failed") { Results = list };
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
