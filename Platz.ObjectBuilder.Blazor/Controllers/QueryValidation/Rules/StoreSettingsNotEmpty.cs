using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation.Rules
{
    public class StoreSettingsNotEmpty : IQueryBuilderRule
    {
        public RuleValidationResult Validate(IQueryModel qm)
        {
            if (string.IsNullOrWhiteSpace(qm.StoreParameters.DataService))
            {
                return new RuleValidationResult($"Settings Parameter 'DataService' cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.Namespace))
            {
                return new RuleValidationResult($"Settings Parameter 'Namespace' cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.QueryName))
            {
                return new RuleValidationResult($"Settings Parameter 'QueryName' cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.QueryReturnType))
            {
                return new RuleValidationResult($"Settings Parameter 'QueryReturnType' cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(qm.StoreParameters.StoreDataPath))
            {
                return new RuleValidationResult($"Settings Parameter 'StoreDataPath' cannot be empty");
            }

            return null;
        }
    }
}
