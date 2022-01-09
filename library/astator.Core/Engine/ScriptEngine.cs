using astator.Core.Script;
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
        /// 解析cs字符串
        /// </summary>
        public void ParseScript(string text, string path = default)
        {
            this.trees.Add(CSharpSyntaxTree.ParseText(text, path: path, encoding: Encoding.UTF8));
        }

        /// <summary>
        /// 解析cs文件
        /// </summary>
        /// <param name="path"></param>
        public void ParseScriptFromFile(string path)
        {
            if (path.EndsWith(".cs") || path.EndsWith(".csx"))
            {
                this.trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path, encoding: Encoding.UTF8));
            }
        }

        /// <summary>
        /// 解析csx脚本文件
        /// </summary>
        /// <param name="path"></param>
        public void ParseCsx(string path)
        {
            var lines = File.ReadAllLines(path);
            var directory = Path.GetDirectoryName(path);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("#"))
                {
                    if (line.StartsWith("#r"))
                    {
                        var reference = Path.Combine(directory, line[(line.IndexOf('"') + 1)..line.LastIndexOf('"')]);
                        LoadReference(reference);
                    }
                    else if (line.StartsWith("#load"))
                    {
                        var script = Path.Combine(directory, line[(line.IndexOf('"') + 1)..line.LastIndexOf('"')]);
                        if (script.EndsWith("csx"))
                        {
                            ParseCsx(script);
                        }
                        else if (script.EndsWith("cs"))
                        {
                            ParseScriptFromFile(script);
                        }
                    }
                    lines[i] = lines[i].Replace("#", "//#");
                }
                else
                {
                    var text = string.Join("\r\n", lines);
                    ParseScript(text, path);
                    return;
                }
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
        /// <param name="method">入口方法</param>
        /// <param name="runtime"></param>
        public void Execute(MethodInfo method, dynamic runtime)
        {
            try
            {
                method.Invoke(null, new object[] { runtime });
            }
            catch (Exception ex)
            {
                ScriptLogger.Instance.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 获取入口方法
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetEntryMethod()
        {
            try
            {
                this.assembly.TryGetTarget(out var assembly);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    var methods = type.GetMethods();
                    foreach (var method in methods)
                    {
                        if (Attribute.IsDefined(method, typeof(EntryMethod)))
                        {
                            if (method.IsStatic && method.IsPublic)
                            {
                                return method;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptLogger.Instance.Error(ex.ToString());
            }

            return null;
        }
    }
}