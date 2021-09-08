using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Platz.ObjectBuilder.Blazor.Model;
using Platz.ObjectBuilder.Blazor.Validation;
using Platz.SqlForms;
using Platz.SqlForms.Shared;
using Platz.SqlForms.Shared.DynamicCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IFormBuilderController
    {
        FormBuilderModel Model { get; set; }
        void Change();

        List<RuleValidationResult> Validate(FormBuilderModel model = null);
        List<FieldComponentModel> GetPageFieldComponents();
        void SetActive(FieldComponentModel field);
        void DeleteField(FieldComponentModel field);
        void ReOrderFields(IList<FieldComponentModel> items);
        bool PageActive { get; }
        FieldComponentModel ActiveField { get; }
        List<FieldRuleModel> ActiveFieldRules { get; }
        int SelectedRuleIndex { get; set; }
        void PrepareActiveFieldRulesForEdit();
        Task<List<string>> ValidateDirtyRules(List<FieldRuleModel> rules);
        void ApplyActiveFieldRules(List<FieldRuleModel> rules);

        void SetSchemas(List<StoreSchema> storeSchemas);
        void SetQueries(List<StoreQuery> storeQueries);
        void SetForms(List<StoreForm> storeForms);
        void RefreshDatasources();
        void RefreshQueryParams();
        void RefreshHeaderParams();

        // Toolbar
        void AddTextEdit();
        void AddActionButton();
        void MoveUp();
        void MoveDown();
        void Clear();
        void GenerateFromEntity();

        List<string> GetEntityBindings();
        void SwitchModel(FormBuilderModel model);
        void UpdateFormName(string name);
        List<string> GetAvailableFormReferences();
    }
    public class FormBuilderController : IFormBuilderController
    {
        public const string DS_QUERY_START = "q: ";

        private readonly IBuilderRuleFactory<IFormBuilderRule, FormBuilderModel> _ruleEngine;

        //private List<FieldComponentModel> _fields;
        private List<StoreSchema> _storeSchemas;
        private List<StoreQuery> _storeQueries;
        private List<StoreForm> _storeForms;

        public bool PageActive { get; set; }
        public FieldComponentModel ActiveField { get; set; }
        public List<FieldRuleModel> ActiveFieldRules { get; private set; } = new List<FieldRuleModel>();
        public int SelectedRuleIndex { get; set; } = -1;

        public FormBuilderModel Model { get; set; }

        public FormBuilderController(IBuilderRuleFactory<IFormBuilderRule, FormBuilderModel> ruleEngine)
        {
            _ruleEngine = ruleEngine;
            // ToDo: remove this simulation
            Model = new FormBuilderModel();
            //Model.Fields.Add(new FieldComponentModel { Name = "Name", Binding = "$.Name", ComponentType = FieldComponentType.TextEdit, StoreField = new StoreFormField() });
            //Model.Fields.Add(new FieldComponentModel { Name = "Type", Binding = "$.Type", ComponentType = FieldComponentType.Dropdown, StoreField = new StoreFormField() });
            //Model.Fields.Add(new FieldComponentModel { Name = "Created", Binding = "$.CreatedDate", ComponentType = FieldComponentType.DateEdit, StoreField = new StoreFormField() });
            
            ApplySortOrder();
        }

        public void SwitchModel(FormBuilderModel model)
        {
            Model = model;
        }

        public List<FieldComponentModel> GetPageFieldComponents()
        {
            return Model.Fields;
        }

        public void ReOrderFields(IList<FieldComponentModel> items)
        {
            // Actually Dropzone compnents reorders our _fields property
            Model.Fields = items.ToList();
            ApplySortOrder();
        }

        private void ApplySortOrder()
        {
            int i = 0;
            Model.Fields.ForEach(f => f.Order = i++);
        }

        public void SetActive(FieldComponentModel field)
        {
            Model.Fields.ForEach(f => f.Active = false);
            ActiveField = null;

            if (field == null || Model.Fields.IndexOf(field) == -1)
            {
                PageActive = true;
                return;
            }

            PageActive = false;
            field.Active = true;
            ActiveField = field;
        }

        public void SetSchemas(List<StoreSchema> storeSchemas)
        {
            _storeSchemas = storeSchemas;
            Model.Schemas = _storeSchemas.Select(x => x.Name).ToList();
        }

        public void SetQueries(List<StoreQuery> storeQueries)
        {
            _storeQueries = storeQueries;
            //Model.Datasources = _storeQueries.Select(x => x.Name).ToList();
        }

        public void SetForms(List<StoreForm> storeForms)
        {
            _storeForms = storeForms;
        }

        public void RefreshDatasources()
        {
            Model.Datasources = new List<string>();

            if (string.IsNullOrWhiteSpace(Model.Schema))
            {
                return;
            }

            Model.Datasources.AddRange(_storeQueries.Where(x => x.SchemaName == Model.Schema).Select(x => $"{DS_QUERY_START}{x.Name}").OrderBy(x => x));

            Model.Datasources.AddRange(_storeSchemas.First(x => x.Name == Model.Schema).Definitions.Keys.OrderBy(x => x));

            if (Model.IsListForm)
            {

            }
            else
            {

            }
        }

        public void Change()
        {
            ApplySortOrder();
            Model.IsDirty = true;
        }

        public void AddTextEdit()
        {
            var f = new FieldComponentModel { ComponentType = FieldComponentType.TextEdit };
            Model.Fields.Add(f);
            Change();
        }

        public void AddActionButton()
        {
            var f = new FieldComponentModel { ComponentType = FieldComponentType.ActionButton };
            f.StoreButton.Action = ButtonActionTypes.Submit.ToString();
            f.StoreButton.Text = f.StoreButton.Action;
            Model.Fields.Add(f);
            Change();
        }

        public void MoveUp()
        {
            if (ActiveField != null)
            {
                var i = Model.Fields.IndexOf(ActiveField);

                if (i > 0)
                {
                    Model.Fields.RemoveAt(i);
                    Model.Fields.Insert(i - 1, ActiveField);
                    Change();
                }
            }
        }

        public void MoveDown()
        {
            if (ActiveField != null)
            {
                var i = Model.Fields.IndexOf(ActiveField);

                if (i < Model.Fields.Count-1)
                {
                    Model.Fields.RemoveAt(i);
                    Model.Fields.Insert(i + 1, ActiveField);
                    Change();
                }
            }
        }

        public void Clear()
        {
            Model.Fields.Clear();
            Change();
        }

        public void GenerateFromEntity()
        {
            throw new NotImplementedException();
            Change();
        }

        public List<string> GetEntityBindings()
        {
            if (string.IsNullOrWhiteSpace(Model.Datasource))
            {
                return new List<string>();
            }

            if (Model.Datasource.StartsWith(DS_QUERY_START))
            {
                var name = Model.Datasource.Replace(DS_QUERY_START, "");
                var q = _storeQueries.First(x => x.SchemaName == Model.Schema && x.Name == name);
                var result = q.Query.Fields.Values.Select(f => $"$.{f.FieldAlias ?? f.Field.FieldName}").ToList();
                return result;
            }
            else
            {
                var t = _storeSchemas.First(x => x.Name == Model.Schema).Definitions[Model.Datasource];
                var result = t.Properties.Values.Select(p => $"$.{p.Name}").ToList();
                return result;
            }
        }

        public void DeleteField(FieldComponentModel field)
        {
            SetActive(null);
            Model.Fields.Remove(field);
            Change();
        }

        public void UpdateFormName(string name)
        {
            Model.Name = name;
            Change();
        }

        public void PrepareActiveFieldRulesForEdit()
        {
            ActiveField.Rules.CopyListTo(ActiveFieldRules);
            SelectedRuleIndex = -1;
        }

        #region Validation
        // ToDo: remove all codegeneration and compilation logic to a dedicated specialized class
        private static Assembly SystemRuntime = Assembly.Load(new AssemblyName("System.Runtime"));

        public async Task<List<string>> ValidateDirtyRules(List<FieldRuleModel> rules)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            AddModelCode(sb);

            sb.AppendLine(@"
    public class FieldRules
    {");

            foreach (var rule in rules.Where(r => r.IsDirty))
            {
                AddRuleCode(sb, rule);
            }
            sb.AppendLine(@"
    }");

            var codeText = sb.ToString();

            var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(codeText) },
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
                if (!engine.CompilationResult.Success)
                { 
                    foreach (var err in engine.CompilationResult.Diagnostics)
                    {
                        result.Add(err.ToString());
                    }
                }
            }

            return result;
        }

        private void AddModelCode(StringBuilder sb)
        {

            sb.AppendLine($@"
    using System;
    using Platz.SqlForms;");

            if (Model.Datasource.StartsWith(DS_QUERY_START))
            {
                var ds = Model.Datasource.Replace(DS_QUERY_START, "");
                var q = _storeQueries.First(x => x.SchemaName == Model.Schema && x.Name == ds);
                AppendQueryReturnType(sb, _storeSchemas.First(x => x.Name == Model.Schema), q);
            }
            else
            {
                JsonStoreSchemaParser parser = new JsonStoreSchemaParser();
                var className = Model.Datasource.Replace(DS_QUERY_START, "");
                var table = _storeSchemas.First(x => x.Name == Model.Schema).Definitions[Model.Datasource];

                sb.AppendLine($@"
    public class {className}
    {{");
                foreach (var property in table.Properties.Values)
                {
                    sb.Append(@$"
        public {parser.DesignShemaTypeToCSharp(property)} {property.Name} {{ get; set; }}");
                }

                sb.AppendLine(@"
    }");
            }
        }

        private void AddRuleCode(StringBuilder sb, FieldRuleModel rule)
        {
            var className = Model.Datasource.Replace(DS_QUERY_START, "");

            sb.AppendLine($@"
        public FormRuleResult {rule.Name}(RuleArgs<{className}> a)
        {{");

            sb.AppendLine(rule.Code);

            sb.AppendLine(@"
        }");

        }

        // ToDo: refactor!!! This is a copypaste from EntityFrameworkQueryGenerator
        private void AppendQueryReturnType(StringBuilder sb, StoreSchema schema, StoreQuery query)
        {
            var subQueries = query.Query.SubQueries;

            sb.Append(@$"
    public  class {query.ReturnTypeName}
    {{");

            foreach (var field in query.Query.Fields.Values.Where(f => f.IsOutput))
            {
                var table = query.Query.Tables[field.Field.ObjectAlias];

                if (table.IsSubQuery)
                {
                    string originalPropertyName = field.FieldAlias;

                    while (table.IsSubQuery)
                    {
                        var sq = subQueries[table.TableName];
                        var sqf = sq.Fields[originalPropertyName];
                        originalPropertyName = sqf.Field.FieldName;
                        var sqt = sq.Tables[sqf.Field.ObjectAlias];
                        table = sqt;
                    }

                    var definition = schema.Definitions[table.TableName];
                    var property = definition.Properties[originalPropertyName];

                    sb.Append(@$"
        public {property.Type} {field.FieldAlias} {{ get; set; }}");
                }
                else
                {
                    var definition = schema.Definitions[table.TableName];
                    var property = definition.Properties[field.Field.FieldName];

                    sb.Append(@$"
        public {property.Type} {field.FieldAlias} {{ get; set; }}");

                }
            }

            sb.Append(@$"
    }}");

        }

        #endregion

        public void ApplyActiveFieldRules(List<FieldRuleModel> rules)
        {
            rules.ForEach(r => r.IsDirty = false);
            rules.CopyListTo(ActiveField.Rules);
        }

        public List<string> GetAvailableFormReferences()
        {
            var result = _storeForms.Where(m => m.Name != Model.Name && m.Name != Model.OriginalName).Select(f => f.Name).ToList();
            return result;
        }

        public void RefreshQueryParams()
        {
            Model.QueryParams.Clear();

            if (!string.IsNullOrWhiteSpace(Model.Datasource) && Model.Datasource.StartsWith(DS_QUERY_START))
            {
                var name = Model.Datasource.Replace(DS_QUERY_START, "");
                var q = _storeQueries.First(x => x.SchemaName == Model.Schema && x.Name == name);
                Model.QueryParams = q.Query.Parameters.Values.OrderBy(x => x.Order).Select(x => x.Name).ToList();
            }
        }

        public void RefreshHeaderParams()
        {
            Model.HeaderParams.Clear();

            if (!string.IsNullOrWhiteSpace(Model.PageHeaderForm))
            {
                var form = _storeForms.First(f => f.Name == Model.PageHeaderForm);
                Model.HeaderParams = form.PageParameters.Values.OrderBy(x => x.Order).Select(x => x.Name).ToList();
            }
        }

        public List<RuleValidationResult> Validate(FormBuilderModel model = null)
        {
            return _ruleEngine.ValidateAllRules(model ?? Model);
        }
    }
}
