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
                return new RuleValidationResult("Select clause should contain at list one output property");
            }

            return null;
        }
    }
}
