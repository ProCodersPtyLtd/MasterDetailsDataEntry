using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Platz.SqlForms.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms.Shared.DynamicCode
{
    public class DynamicCodeEngine : IDynamicCodeEngine, IDisposable
    {
        private readonly CollectibleAssemblyLoadContext _context;
        private readonly MemoryStream _ms;
        private Assembly _assembly;

        public DynamicCodeEngine()
        { 
            _ms = new MemoryStream();
            _context = new CollectibleAssemblyLoadContext();
        }

        public DynamicCodeEngine(Compilation compilation) : this()
        {
            CreateCodeEngine(compilation);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CreateCodeEngine(Compilation compilation)
        {
            var cr = compilation.Emit(_ms);
            _ms.Seek(0, SeekOrigin.Begin);
            _assembly = _context.LoadFromStream(_ms);

            //var greetMethod = type.GetMethod("Hello");
            //var result = (int)greetMethod.Invoke(instance, new object[] { i });
            //Console.WriteLine(result);
        }

        public Type GetType(string typeName)
        {
            var type = _assembly.GetType(typeName);
            return type;
        }

        public object CreateInstance(string typeName)
        {
            var type = _assembly.GetType(typeName);

            var instance = Activator.CreateInstance(type);
            return instance;
        }

        public void Dispose()
        {
            _ms.Dispose();
            _context.Unload();
        }

        private static Assembly SystemRuntime = Assembly.Load(new AssemblyName("System.Runtime"));

        public static Compilation SimpleCompilation(int i)
        {
            var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(@"
                public class Greeter
                {
                    public int Hello(int iteration)
                    {
                        return iteration + 3;
                    }
                }") },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(SystemRuntime.Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation;
        }
    }
}
