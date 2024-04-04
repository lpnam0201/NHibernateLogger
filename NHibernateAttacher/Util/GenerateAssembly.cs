using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateProfilerLibrary.Util
{
    public static class GenerateAssembly
    {
        private static readonly PortableExecutableReference[] References = (PortableExecutableReference[])(object)new PortableExecutableReference[7]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(NameValueCollection).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enum).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Collections")).Location)
        };

        public static Assembly Compile(string sourceNamespace, IList<string> sources, IList<string> libraries)
        {
            var syntaxTrees = new List<SyntaxTree>();

            foreach (var source in sources)
            {
                using (var stream = typeof(GenerateAssembly).Assembly.GetManifestResourceStream($"{sourceNamespace}.{source}"))
                {
                    SourceText sourceText = SourceText.From(stream);
                    var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);

                    syntaxTrees.Add(syntaxTree);
                }
            }

            var portableExecutableReferences = libraries.Select(l => MetadataReference.CreateFromFile(l))
                .Concat(References);

            var compilation = CSharpCompilation.Create("Test", syntaxTrees, portableExecutableReferences,
                options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
            
            using (var stream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream);
                stream.Position = 0;

                return AssemblyLoadContext.Default.LoadFromStream(stream);
            }
        }
    }
}
