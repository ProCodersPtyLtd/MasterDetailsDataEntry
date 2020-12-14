using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation
{
    public interface IObjectBuilderRuleFactory
    {
        List<IObjectBuilderRule> GetAllRules();
        void RegisterRule(IObjectBuilderRule rule);
        List<RuleValidationResult> ValidateAllRules(IQueryModel qm);
    }

    public class ObjectBuilderRuleFactory : IObjectBuilderRuleFactory
    {
        private readonly List<IObjectBuilderRule> _rules;

        public ObjectBuilderRuleFactory()
        {
            _rules = new List<IObjectBuilderRule>();
            var localRules = GetLocalRules();
            _rules.AddRange(localRules);
        }

        public List<IObjectBuilderRule> GetAllRules()
        {
            return _rules;
        }

        public void RegisterRule(IObjectBuilderRule rule)
        {
            if (!_rules.Any(r => r.GetType() == rule.GetType()))
            {
                _rules.Add(rule);
            }
        }

        public List<RuleValidationResult> ValidateAllRules(IQueryModel qm)
        {
            var result = new List<RuleValidationResult>();
            var rules = GetAllRules();

            foreach (var rule in rules)
            {
                var vr = rule.Validate(qm);

                if (vr != null && vr.IsFailed)
                {
                    result.Add(vr);
                }
            }

            return result;
        }

        private List<IObjectBuilderRule> GetLocalRules()
        {
            var result = typeof(ObjectBuilderRuleFactory).Assembly.GetTypes().Where(t => !t.IsInterface && typeof(IObjectBuilderRule).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as IObjectBuilderRule)
                .ToList();

            return result;
        }
    }
}
