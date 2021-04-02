using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation.Rules
{
    public class FromTablesNotEmpty : IObjectBuilderRule
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
