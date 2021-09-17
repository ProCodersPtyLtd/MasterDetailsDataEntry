using Platz.ObjectBuilder.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Engine
{
    public class FormCodeGenerator
    {
        public CodeGenerationSection GenerateRazorPageForm(StoreForm storeForm)
        {
            var result = new CodeGenerationSection() { FileName = storeForm.Name + ".razor.cs" };
            return result;
        }

        public CodeGenerationSection GenerateFormForm(StoreForm storeForm)
        {
            var result = new CodeGenerationSection() { FileName = storeForm.Name + ".cs" };
            return result;
        }
    }
}
