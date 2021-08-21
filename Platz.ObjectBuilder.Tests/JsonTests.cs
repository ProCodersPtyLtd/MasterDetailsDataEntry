using Platz.ObjectBuilder.Blazor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests
{
    public class JsonTests
    {
        [Fact]
        public void Test1()
        {
            var json = "{\"Id\":1,\"Name\":\"q\",\"Surname\":\"qq\",\"Phone\":\"qqq\",\"Dob\":null}";
            var e = JsonSerializer.Deserialize(json, typeof(Default.Person));
        }

        [Fact]
        public void CodeInJsonTest()
        {
            var data = new FieldRuleModel();
            data.Code = @"
    using System;
    using Platz.SqlForms;

    public class Person
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime Dob { get; set; }
    }

    public class Rule
    {
        public FormRuleResult CheckCompanyRequired(RuleArgs<Person> a)
        {
            var required = (a.Model.LastName == ""Ford"");
            a.Entity.Property(p => p.FirstName).IsRequired(required).Label(required ? ""Ford Division"": ""Company Name"");
            return null;
        }
    }";
            var json = JsonSerializer.Serialize(data);
            var data2 = JsonSerializer.Deserialize<FieldRuleModel>(json);

            Assert.Equal(data.Code, data2.Code);
        }
    }
}
