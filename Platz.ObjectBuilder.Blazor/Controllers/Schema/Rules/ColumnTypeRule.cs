using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema.Rules
{
    public class ColumnTypeRule : ISchemaBuilderRule
    {
        public RuleValidationResult Validate(DesignSchema model)
        {
            foreach (var t in model.Tables)
            {
                foreach (var c in t.Columns)
                {
                    if (!c.IsEmpty() && string.IsNullOrWhiteSpace(c.Type))
                    {
                        return new RuleValidationResult($"Table '{t.Name}' column name '{c.Name}' must have data type.");
                    }
                }
            }

            return null;
        }
    }
}
