using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;

namespace astator.Engine
{
    public class ScriptEngine
    {
        private static IEnumerable<MetadataReference> References;

        private Domain alc;

        private WeakReference<Assembly> assembly;

        private List<SyntaxTree> trees = new();

        public ScriptEngine()
        {
            this.alc = new Domain();

            if (References is null)
            {
                References = AssemblyLoadContext.Default.Assemblies.Select(x => MetadataReference.CreateFromFile(x.Location));
            }
        }

        public void LoadScript(string path)
        {
            if (path.EndsWith(".cs"))
            {
                this.trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path, encoding: Encoding.UTF8));
            }
        }

        public void LoadFromAssemblyPath(string path)
        {
            if (path.EndsWith(".dll"))
            {
                this.alc.LoadFromAssemblyPath(path);
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
                syntaxTrees: this.trees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug))
                .AddReferences(References);

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

        public void Execute(string mainType, dynamic runtime)
        {
            this.assembly.TryGetTarget(out var assembly);
            var type = assembly.GetType(mainType);
            if (type is null)
            {

            }
            dynamic obj = Activator.CreateInstance(type);
            if (obj is not null)
            {
                obj.Main(runtime);
            }
        }
    }
}