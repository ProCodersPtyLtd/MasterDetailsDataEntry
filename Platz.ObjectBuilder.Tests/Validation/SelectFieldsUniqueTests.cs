﻿using Platz.ObjectBuilder.Blazor.Controllers;
using Platz.ObjectBuilder.Blazor.Controllers.Validation;
using Platz.ObjectBuilder.Blazor.Controllers.Validation.Rules;
using Platz.ObjectBuilder.Schema;
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
            var qc = new EntityFrameworkQueryController(null);
            var t1 = new QueryFromTable { Alias = "t", StoreDefinition = new StoreDefinition { Name = "table1" } };
            var p1 = new QueryFromProperty { Alias = "id", StoreProperty = new StoreProperty { Name = "id" } };
            var p2 = new QueryFromProperty { StoreProperty = new StoreProperty { Name = "id" } };
            var p3 = new QueryFromProperty { Alias = "myid", StoreProperty = new StoreProperty { Name = "id" } };
            qc.AddSelectionProperty(t1, p1);
            qc.AddSelectionProperty(t1, p2);
            qc.AddSelectionProperty(t1, p3);
            var rule = new SelectFieldsUnique();
            var vr = rule.Validate(qc);

            Assert.NotNull(vr);
            Assert.True(vr.IsFailed);
            Assert.Contains("id", vr.Message);
            Assert.DoesNotContain("myid", vr.Message);
        }

        [Fact]
        public void SelectFieldsUniquePassTest()
        {
            var qc = new EntityFrameworkQueryController(null);
            var t1 = new QueryFromTable { Alias = "t", StoreDefinition = new StoreDefinition { Name = "table1" } };
            var p1 = new QueryFromProperty { Alias = "id2", StoreProperty = new StoreProperty { Name = "id" } };
            var p2 = new QueryFromProperty { StoreProperty = new StoreProperty { Name = "id" } };
            var p3 = new QueryFromProperty { Alias = "myid", StoreProperty = new StoreProperty { Name = "id" } };
            qc.AddSelectionProperty(t1, p1);
            qc.AddSelectionProperty(t1, p2);
            qc.AddSelectionProperty(t1, p3);
            var rule = new SelectFieldsUnique();
            var vr = rule.Validate(qc);

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
