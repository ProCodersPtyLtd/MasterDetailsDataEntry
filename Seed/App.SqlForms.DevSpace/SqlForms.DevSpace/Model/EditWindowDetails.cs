using Platz.ObjectBuilder.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlForms.DevSpace.Model
{
    public class EditWindowDetails
    {
        public EditWindowType Type { get; set; }

        public IStoreObject StoreObject { get; set; }

        public IStoreObjectDetails StoreObjectDetails { get; set; }
    }

    public enum EditWindowType
    {
        Unknown = 0,
        Output = 1,
        CodePreview = 2,
        ProjectSettings = 10,
        Schema = 20,
        Query = 30,
        Form = 40
    }

    public class SpecialWindowStoreObject : IStoreObject
    {
        public string Name { get; set; }
        public bool Validated { get; set; }
        public SpecialWindowContent Content { get; set; }
    }

    public abstract class SpecialWindowContent
    {
    }

    public class CodePreviewSpecialWindowContent : SpecialWindowContent
    {
        public List<CodeGenerationSection> Sections { get; set; } = new List<CodeGenerationSection>();

        public CodePreviewSpecialWindowContent(IEnumerable<CodeGenerationSection> sections)
        {
            Sections.AddRange(sections);
        }
    }

    
}
