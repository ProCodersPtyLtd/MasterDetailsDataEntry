using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class SelectFieldOutputNotEmpty : IQueryBuilderRule
    {
        public RuleValidationResult Validate(IQueryModel qm)
        {
            if (!qm.SelectionProperties.Where(prop => prop.IsOutput).Any())
            {
                return new RuleValidationResult($"Query '{qm.Name}' Select clause should contain at least one output property");
            }

            return null;
        }

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
                var result = new RuleValidationResult($"SelectFieldOutputNotEmpty failed") { Results = list };
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
