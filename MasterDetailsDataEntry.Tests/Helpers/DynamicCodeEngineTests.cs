using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Platz.SqlForms.Shared.DynamicCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MasterDetailsDataEntry.Tests.Helpers
{
    public class DynamicCodeEngineTests
    {
        [Fact]
        public void CompileCodeTest()
        {
            var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(@"
    using System;

    public class Person
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime Dob { get; set; }
    }") },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(SystemRuntime.Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var engine = new DynamicCodeEngine(compilation))
            {
                var type = engine.GetType("Person");
                var person = engine.CreateInstance("Person");
                Assert.NotNull(person);
            }
        }

        [Fact]
        public void CompileRuleCodeTest()
        {
            var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(@"
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
    }") },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(SystemRuntime.Location),
                    MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression<>).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Platz.SqlForms.RuleArgs).GetTypeInfo().Assembly.Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var engine = new DynamicCodeEngine(compilation))
            {
                var rule = engine.CreateInstance("Rule");
                Assert.NotNull(rule);
            }
        }

        private static Assembly SystemRuntime = Assembly.Load(new AssemblyName("System.Runtime"));
    }
}
