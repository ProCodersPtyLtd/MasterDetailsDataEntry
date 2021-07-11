using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class GroupBySelectFieldHavingAllowed : SubQueryBuilderRuleBase
    {
        public override RuleValidationResult Validate(IQueryModel qm)
        {
            var emptyOutputs = qm.SelectionProperties.Where(s => 
                    !string.IsNullOrWhiteSpace(s.Having) &&
                    (string.IsNullOrWhiteSpace(s.GroupByFunction) || s.GroupByFunction == "Where")
                ).ToList();

            if (emptyOutputs.Any())
            {
                var badFields = emptyOutputs.Select(f => f.OutputName);

                var result = new RuleValidationResult(
                    GetType().Name, 
                    $"Query '{qm.Name}' cannot have Group By Having not included in Group By clause or aggregate functions: {string.Join(", ", badFields)}");
                    
                return result;
            }

            return null;
        }
    }
}
