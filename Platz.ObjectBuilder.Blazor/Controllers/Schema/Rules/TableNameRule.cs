using Platz.ObjectBuilder.Blazor.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Controllers.Schema.Rules
{
    public class TableNameRule : ISchemaBuilderRule
    {
        public RuleValidationResult Validate(DesignSchema model)
        {
            foreach(var t in model.Tables)
            {
                //if (!t.Name.All(c => char.IsLetterOrDigit(c) || c == '_'))
                //{
                //    return new RuleValidationResult($"Table name '{t.Name}' must contain only letters, digits, underscore");
                //}

                //if (string.IsNullOrWhiteSpace(t.Name))
                //{
                //    return new RuleValidationResult($"Table name '{t.Name}' is empty");
                //}

                //if (!char.IsLetter(t.Name[0]) && t.Name[0] != '_')
                //{
                //    return new RuleValidationResult($"Table name '{t.Name}' can start only from letter or underscore");
                //}
                var result = CheckObjectName("Table", t.Name);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static RuleValidationResult CheckObjectName(string objectType, string name)
        {
            if (!name.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                return new RuleValidationResult($"{objectType} name '{name}' must contain only letters, digits, underscore.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return new RuleValidationResult($"{objectType} name '{name}' is empty.");
            }

            if (!char.IsLetter(name[0]) && name[0] != '_')
            {
                return new RuleValidationResult($"{objectType} name '{name}' can start only from letter or underscore.");
            }

            return null;
        }
    }
}
