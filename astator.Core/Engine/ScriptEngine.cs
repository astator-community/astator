using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace astator.Core.Engine
{
    public class ScriptEngine
    {
        private static List<MetadataReference> References;

        private readonly Domain alc;

        private WeakReference<Assembly> assembly;

        private readonly List<SyntaxTree> trees = new();

        private readonly List<MetadataReference> scriptReferences = new();

        public ScriptEngine(string directory)
        {
            this.alc = new Domain();

            if (References is null)
            {
                References = new();
                foreach (var path in Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories))
                {
                    References.Add(MetadataReference.CreateFromFile(path));
                }
            }
        }

        public void LoadScript(string path)
        {
            if (path.EndsWith(".cs"))
            {
                this.trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path, encoding: Encoding.UTF8));
            }
        }

        public void LoadReference(string path)
        {
            if (path.EndsWith(".dll"))
            {
                this.scriptReferences.Add(MetadataReference.CreateFromFile(path));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void UnExecute()
        {
            this.alc.Unload();
        }

        public EmitResult Compile()
        {
            var assemblyName = Path.GetRandomFileName();
            var compilation = CSharpCompilation.Create(
                assemblyName,

                references: References.Concat(this.scriptReferences).ToList(),
                syntaxTrees: this.trees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            using var dll = new MemoryStream();
            using var pdb = new MemoryStream();
            var result = compilation.Emit(dll, pdb);

            if (result.Success)
            {
                dll.Seek(0, SeekOrigin.Begin);
                pdb.Seek(0, SeekOrigin.Begin);
                this.assembly = new WeakReference<Assembly>(this.alc.LoadFromStream(dll, pdb));
            }
            return result;
        }

        public void Execute(string entryType, dynamic runtime)
        {
            try
            {
                this.assembly.TryGetTarget(out var assembly);
                var type = assembly.GetType(entryType);
                dynamic obj = Activator.CreateInstance(type);
                obj.Main(runtime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}