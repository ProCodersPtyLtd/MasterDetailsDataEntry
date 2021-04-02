using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema.Rules
{
    public class ColumnNameRule : ISchemaBuilderRule
    {
        public RuleValidationResult Validate(DesignSchema model)
        {
            foreach(var t in model.Tables)
            {
                foreach (var c in t.Columns)
                {
                    var result = TableNameRule.CheckObjectName($"Table '{t.Name}' column", c.Name);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
