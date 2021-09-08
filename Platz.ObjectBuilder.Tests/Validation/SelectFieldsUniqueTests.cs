using Platz.ObjectBuilder.Blazor.Controllers;
using Platz.ObjectBuilder.Blazor.Validation;
using Platz.ObjectBuilder.Blazor.Validation.Rules;
using Platz.ObjectBuilder.Schema;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests.Validation
{
    public class SelectFieldsUniqueTests
    {
        [Fact]
        public void SelectFieldsUniqueFailTest()
        {
            var qc = new QueryController(null);
            var t1 = new QueryFromTable(new StoreDefinition { Name = "table1" }) { Alias = "t" };
            var p1 = new QueryFromProperty { Alias = "id", OriginalStoreProperty = new StoreProperty { Name = "x1" } };
            var p2 = new QueryFromProperty { OriginalStoreProperty = new StoreProperty { Name = "id" } };
            var p3 = new QueryFromProperty { Alias = "myid", OriginalStoreProperty = new StoreProperty { Name = "x2" } };
            qc.AddSelectionProperty(t1, p1);
            qc.AddSelectionProperty(t1, p2);
            qc.AddSelectionProperty(t1, p3);
            var rule = new SelectFieldsUnique();
            var vr = rule.Validate(qc.MainQuery);

            Assert.NotNull(vr);
            Assert.Equal(ValidationResultTypes.Error, vr.Type);
            Assert.Contains("id", vr.Message);
            Assert.DoesNotContain("myid", vr.Message);
        }

        [Fact]
        public void SelectFieldsUniquePassTest()
        {
            var qc = new QueryController(null);
            var t1 = new QueryFromTable(new StoreDefinition { Name = "table1" }) { Alias = "t" };
            var p1 = new QueryFromProperty { Alias = "id2", OriginalStoreProperty = new StoreProperty { Name = "id" } };
            var p2 = new QueryFromProperty { OriginalStoreProperty = new StoreProperty { Name = "id" } };
            var p3 = new QueryFromProperty { Alias = "myid", OriginalStoreProperty = new StoreProperty { Name = "id" } };
            qc.AddSelectionProperty(t1, p1);
            qc.AddSelectionProperty(t1, p2);
            qc.AddSelectionProperty(t1, p3);
            var rule = new SelectFieldsUnique();
            var vr = rule.Validate(qc.MainQuery);

            Assert.Null(vr);
        }

        [Fact]
        public void RuleValidationResultAutoRuleNameTest()
        {
            var vr = new RuleValidationResult("message");
            Assert.Equal(GetType().Name, vr.RuleName);
        }
    }
}
