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

    /// <summary>
    /// 脚本编译执行引擎
    /// </summary>
    public class ScriptEngine
    {
        /// <summary>
        /// sdk引用
        /// </summary>
        private static List<MetadataReference> References;

        /// <summary>
        /// 动态域
        /// </summary>
        private readonly Domain alc;

        /// <summary>
        /// 编译程序集
        /// </summary>
        private WeakReference<Assembly> assembly;

        /// <summary>
        /// 解析语法树
        /// </summary>
        private readonly List<SyntaxTree> trees = new();

        /// <summary>
        /// 脚本程序集引用
        /// </summary>
        private readonly List<MetadataReference> scriptReferences = new();


        public ScriptEngine()
        {
            this.alc = new Domain();

            var sdkDir = Android.App.Application.Context.GetExternalFilesDir("Sdk").ToString();

            if (References is null)
            {
                References = new();
                foreach (var path in Directory.GetFiles(sdkDir, "*.dll", SearchOption.AllDirectories))
                {
                    References.Add(MetadataReference.CreateFromFile(path));
                }
            }
        }

        /// <summary>
        /// 解析cs文件
        /// </summary>
        /// <param name="path"></param>
        public void ParseScript(string path)
        {
            if (path.EndsWith(".cs"))
            {
                this.trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path, encoding: Encoding.UTF8));
            }
        }

        /// <summary>
        /// 加载引用程序集
        /// </summary>
        /// <param name="path"></param>
        public void LoadReference(string path)
        {
            if (path.EndsWith(".dll"))
            {
                this.scriptReferences.Add(MetadataReference.CreateFromFile(path));
            }
        }

        /// <summary>
        /// 动态域卸载
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void UnExecute()
        {
            this.alc.Unload();
        }

        /// <summary>
        /// 编译
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 编译到文件
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public EmitResult Compile(string outputPath)
        {
            var assemblyName = Path.GetRandomFileName();
            var compilation = CSharpCompilation.Create(
                assemblyName,
                references: References.Concat(this.scriptReferences).ToList(),
                syntaxTrees: this.trees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

            using var dll = new MemoryStream();
            var result = compilation.Emit(dll);

            if (result.Success)
            {
                dll.Seek(0, SeekOrigin.Begin);
                using var fs = new FileStream(outputPath, FileMode.OpenOrCreate);
                dll.CopyTo(fs);
            }

            return result;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="entryType">入口类名称</param>
        /// <param name="runtime">scriptRuntime</param>
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