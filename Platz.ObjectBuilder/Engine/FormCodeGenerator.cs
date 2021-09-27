using Platz.ObjectBuilder.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Engine;
public class FormCodeGenerator
{
    //private readonly IProjectLoader _loader;

    //public FormCodeGenerator(IProjectLoader projectLoader)
    //{
    //    _loader = projectLoader;
    //}

    public CodeGenerationSection GenerateEditRazorPageForm(StoreForm form, StoreCodeGeneratorContext ctx)
    {
        var result = new CodeGenerationSection() { FileName = form.Name + ".razor.cs" };
        var sb = new StringBuilder();
        var psb = new StringBuilder();
        var fpsb = new StringBuilder();
        var comma = "";

        foreach (var p in form.PageParameters)
        {
            string pt = "";

            switch (p.DataType)
            {
                case "int":
                    pt = ":int";
                    break;
            }

            psb.Append($"/{{{p.Name}{pt}}}");
            fpsb.Append($"{comma}{p.Name}");
            comma = ", ";
        }

        sb.AppendLine(@$"@page ""/{form.RoutingPath}{psb.ToString()}""");

        sb.AppendLine(@$"@using Platz.SqlForms");

        sb.AppendLine(@$"@using {form.Namespace}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(form.Caption))
        {
            sb.AppendLine(@$"<h1>{form.Caption}</h1>");
            sb.AppendLine();
        }

        // Header
        if (!string.IsNullOrWhiteSpace(form.PageHeaderForm))
        {
            var readOnly = "";

            if (form.PageHeaderFormReadOnly)
            {
                readOnly = @"ReadOnly=""true"" ";
            }

            sb.AppendLine($@"<FormDynamicEditComponent TForm=""{form.PageHeaderForm}"" FormParameters=""GetHeaderParameters()"" {readOnly}/> ");
        }

        sb.AppendLine($@"<FormDynamicEditComponent TForm=""{form.Name}"" FormParameters=""GetParameters()"" /> ");

        sb.AppendLine(@"
@code {");

        foreach (var p in form.PageParameters)
        {
            sb.AppendLine(@$"    [Parameter]");
            sb.AppendLine(@$"    public {p.DataType} {p.Name} {{ get; set; }}");
        }

        sb.AppendLine();
        sb.AppendLine($@"    private FormParameter[] GetParameters()");
        sb.AppendLine($@"    {{");
        sb.AppendLine($@"        return new FormParameter[]");
        sb.AppendLine($@"        {{");

        foreach (var p in form.PageParameters)
        {
            sb.AppendLine(@$"           new FormParameter(""{p.Name}"", {p.Name}),");
        }

        sb.AppendLine($@"        }}");
        sb.AppendLine($@"    }}");

        // Header Parameters
        if (!string.IsNullOrWhiteSpace(form.PageHeaderForm))
        {
            sb.AppendLine();
            sb.AppendLine($@"    private FormParameter[] GetHeaderParameters()");
            sb.AppendLine($@"    {{");
            sb.AppendLine($@"        return new FormParameter[]");
            sb.AppendLine($@"        {{");

            var headerForm = ctx.Forms[form.PageHeaderForm];

            foreach (var headerParameter in headerForm.PageParameters)
            {
                var parameter = form.PageParameters.First(p => p.HeaderFormParameterMapping == headerParameter.Name);
                sb.AppendLine(@$"           new FormParameter(""{parameter.Name}"", {parameter.Name}),");
            }

            sb.AppendLine($@"        }}");
            sb.AppendLine($@"    }}");
        }

        sb.AppendLine(@"}");

        result.Code = sb.ToString();
        return result;
    }

    public CodeGenerationSection GenerateEditForm(StoreForm form, StoreCodeGeneratorContext ctx)
    {
        var result = new CodeGenerationSection() { FileName = form.Name + ".cs" };
        var sb = new StringBuilder();

        sb.AppendLine(@$"using Platz.SqlForms;");
        sb.AppendLine();

        sb.AppendLine(@$"namespace {form.Namespace};");

        var schema = ctx.Schemas[form.Schema];

        sb.Append(@$"
public class {form.Name} : DynamicEditFormBase<{schema.DbContextName}>
{{");
        sb.Append(@$"
    protected override void Define(DynamicFormBuilder builder)
    {{");
        sb.Append(@$"
        builder.Entity<{form.Datasource}>(e =>
        {{");

        // Fields
        foreach (var field in form.Fields.OrderBy(f => f.Order))
        {
            sb.AppendLine();
            sb.Append($"            e.Property(p => p.{field.BindingProperty.Replace("$.", "")})");
            sb.Append(@$".Label(""{field.Label}"")");

            if (field.PrimaryKey == true)
            {
                sb.Append(".IsPrimaryKey()");
            }

            if (field.Unique == true)
            {
                sb.Append(".IsUnique()");
            }

            if (field.Required == true)
            {
                sb.Append(".IsRequired()");
            }

            if (field.ReadOnly == true)
            {
                sb.Append(".IsReadOnly()");
            }

            if (field.Hidden == true)
            {
                sb.Append(".IsHidden()");
            }

            if (!string.IsNullOrWhiteSpace(field.Format))
            {
                sb.Append(@$".Format(""{field.Format}"")");
            }

            foreach (var rule in field.Rules)
            {
                sb.Append($".Rule({rule.Name})");
            }

            sb.Append(";");
        }

        // Buttons
        foreach (var btn in form.ActionButtons.OrderBy(f => f.Order))
        {
            sb.AppendLine();
            sb.Append($"            e.DialogButton(ButtonActionTypes.{btn.Action}");

            if (!string.IsNullOrWhiteSpace(btn.Text))
            {
                sb.Append(@$", text: ""{btn.Text}""");
            }

            if (!string.IsNullOrWhiteSpace(btn.Hint))
            {
                sb.Append(@$", hint: ""{btn.Hint}""");
            }

            if (!string.IsNullOrWhiteSpace(btn.NavigationTargetForm))
            {
                var ps = new StringBuilder();
                var targetForm = ctx.Forms[btn.NavigationTargetForm];
                
                targetForm.PageParameters.OrderBy(p => p.Order).ToList().ForEach(p => 
                {
                    var btnParam = btn.NavigationParameterMapping.First(b => b.Name == p.Name);
                    ps.Append(@$"/{{{btnParam.SupplyingParameterMapping}}}");
                });

                sb.Append(@$", actionLink: ""{btn.NavigationTargetForm}{ps}""");
            }

            sb.Append(");");
        }

        sb.Append(@$"
        }});");
        sb.AppendLine(@$"
    }}");

        // Rules
        foreach (var field in form.Fields.OrderBy(f => f.Order))
        {
            foreach (var rule in field.Rules)
            {
                sb.AppendLine();
                sb.AppendLine($"    public FormRuleResult {rule.Name}(RuleArgs<{form.Datasource}> a)");
                sb.AppendLine($"    {{");
                var tabCode = rule.Code.Replace("\n", "\n        ");
                sb.AppendLine($"        {tabCode}");
                sb.AppendLine($"    }}");
            }
        }

        sb.AppendLine(@$"}}");

        result.Code = sb.ToString();
        return result;
    }
}

public class StoreCodeGeneratorContext
{
    public Dictionary<string, StoreSchema> Schemas { get; set; }
    public Dictionary<string, StoreQuery> Queries { get; set; }
    public Dictionary<string, StoreForm> Forms { get; set; }
}
