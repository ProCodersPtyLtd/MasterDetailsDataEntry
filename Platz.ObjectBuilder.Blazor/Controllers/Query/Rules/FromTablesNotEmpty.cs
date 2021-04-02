using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Validation.Rules
{
    public class FromTablesNotEmpty : IQueryBuilderRule
    {
        public RuleValidationResult Validate(IQueryModel qm)
        {
            if (!qm.FromTables.Any())
            {
                return new RuleValidationResult("From clause should contain at list one table");
            }

            return null;
        }
    }
}
