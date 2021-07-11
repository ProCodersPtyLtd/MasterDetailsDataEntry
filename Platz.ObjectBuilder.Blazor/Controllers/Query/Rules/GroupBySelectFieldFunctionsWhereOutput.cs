using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class GroupBySelectFieldFunctionsWhereOutput : SubQueryBuilderRuleBase
    {
        public override RuleValidationResult Validate(IQueryModel qm)
        {
            if (qm.SelectionProperties.Any(s => s.GroupByFunction == "Group By"))
            {
                var badOutputs = qm.SelectionProperties.Where(s => s.IsOutput && s.GroupByFunction == "Where").ToList();

                if (badOutputs.Any())
                {
                    var badFields = badOutputs.Select(f => f.OutputName);

                    var result = new RuleValidationResult(
                        GetType().Name, 
                        $"Query '{qm.Name}' is a Group By query but it has some output fields included in Where clause: {string.Join(", ", badFields)}");
                    
                    return result;
                }
            }

            return null;
        }
    }
}
