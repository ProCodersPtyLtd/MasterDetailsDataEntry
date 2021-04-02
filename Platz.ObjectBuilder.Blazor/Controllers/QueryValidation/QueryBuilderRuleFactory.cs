using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation
{
    public interface IQueryBuilderRuleFactory
    {
        List<IQueryBuilderRule> GetAllRules();
        void RegisterRule(IQueryBuilderRule rule);
        List<RuleValidationResult> ValidateAllRules(IQueryModel qm);
    }

    public class QueryBuilderRuleFactory : IQueryBuilderRuleFactory
    {
        private readonly List<IQueryBuilderRule> _rules;

        public QueryBuilderRuleFactory()
        {
            _rules = new List<IQueryBuilderRule>();
            var localRules = GetLocalRules();
            _rules.AddRange(localRules);
        }

        public List<IQueryBuilderRule> GetAllRules()
        {
            return _rules;
        }

        public void RegisterRule(IQueryBuilderRule rule)
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

        private List<IQueryBuilderRule> GetLocalRules()
        {
            var result = typeof(QueryBuilderRuleFactory).Assembly.GetTypes().Where(t => !t.IsInterface && typeof(IQueryBuilderRule).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as IQueryBuilderRule)
                .ToList();

            return result;
        }
    }
}
