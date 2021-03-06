﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platz.ObjectBuilder.Schema;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class GroupBySelectFieldFunctions : SubQueryBuilderRuleBase
    {
        public override RuleValidationResult Validate(IQueryModel qm)
        {
            //if (qm.SelectionProperties.Any(s => !string.IsNullOrWhiteSpace(s.GroupByFunction)))
            if (qm.SelectionProperties.Any(s => s.GroupByFunction == "Group By"))
            {
                var emptyOutputs = qm.SelectionProperties.Where(s => s.IsOutput && string.IsNullOrWhiteSpace(s.GroupByFunction)).ToList();

                if (emptyOutputs.Any())
                {
                    var emptyFields = emptyOutputs.Select(f => f.OutputName);

                    var result = new RuleValidationResult(
                        GetType().Name, 
                        $"Query '{qm.Name}' is a Group By query but it has some output fields not included in Group By clause and without aggregate functions: {string.Join(", ", emptyFields)}");
                    
                    return result;
                }
            }

            return null;
        }
    }
}
