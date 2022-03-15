using astator.Core.Script;
using astator.NugetManager;
using astator.TipsView;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        private static List<MetadataReference> References => SdkReferences.References;

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

        /// <summary>
        /// 项目根目录
        /// </summary>
        private readonly string rootDir;

        private readonly List<string> DefaultAssemblyNames = new();


        public ScriptEngine(string rootDir)
        {
            this.rootDir = rootDir;
            this.alc = new Domain();

            foreach (var assembly in AssemblyLoadContext.Default.Assemblies)
            {
                this.DefaultAssemblyNames.Add(assembly.GetName().Name);
            }
        }

        /// <summary>
        /// 解析cs文件
        /// </summary>
        /// <param name="path"></param>
        private void ParseCSFile(string path)
        {
            if (path.EndsWith(".cs"))
            {
                this.trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path, encoding: Encoding.UTF8));
            }
        }

        /// <summary>
        /// 解析所有cs文件
        /// </summary>
        public void ParseAllCS()
        {
            TipsViewImpl.ChangeTipsText("正在解析cs文件...");

            var scripts = Directory.GetFiles(this.rootDir, "*.cs", SearchOption.AllDirectories);

            foreach (var script in scripts)
            {
                ParseCSFile(script);
            }
        }


        /// <summary>
        /// 加载引用程序集
        /// </summary>
        /// <param name="path"></param>
        public bool LoadReference(string path)
        {
            if (path.StartsWith("."))
            {
                path = Path.Combine(this.rootDir, path);
            }
            if (!path.EndsWith(".dll"))
            {
                ScriptLogger.Error($"加载dll失败, 非dll文件: {path}");
                return false;
            }
            if (!File.Exists(path))
            {
                ScriptLogger.Error($"加载dll失败, 文件不存在: {path}");
                return false;
            }
            var ReferencesIsAdd = Android.App.Application.Context.PackageName.Equals(Globals.AstatorPackageName);
            if (ReferencesIsAdd) this.scriptReferences.Add(MetadataReference.CreateFromFile(path));

            var name = AssemblyName.GetAssemblyName(path).Name;
            if (!this.DefaultAssemblyNames.Contains(name))
            {
                this.alc.LoadFromAssemblyPath(path);
            }

            return true;
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
            TipsViewImpl.ChangeTipsText("正在编译...");

            var assemblyName = Path.GetRandomFileName();
            var compilation = CSharpCompilation.Create(
                assemblyName,
                references: References.Concat(this.scriptReferences).ToList(),
                syntaxTrees: this.trees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug));

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
            TipsViewImpl.ChangeTipsText("正在编译...");

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

                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                }

                using var fs = new FileStream(outputPath, FileMode.OpenOrCreate);
                dll.CopyTo(fs);
            }

            return result;
        }

        public bool LoadAssemblyFromPath()
        {
            try
            {
                TipsViewImpl.ChangeTipsText("正在加载程序集...");
                var dllPath = Path.Combine(this.rootDir, "compile.dll");
                if (!File.Exists(dllPath))
                {
                    ScriptLogger.Error($"dll路径不存在: {dllPath}");
                    return false;
                }

                this.assembly = new WeakReference<Assembly>(this.alc.LoadFromAssemblyPath(dllPath));


                TipsViewImpl.ChangeTipsText("正在加载引用程序集...");
                var refDir = Path.Combine(this.rootDir, "ref");
                if (Directory.Exists(refDir))
                {
                    var refs = Directory.EnumerateFiles(refDir, "*.dll", SearchOption.AllDirectories);
                    if (refs.Any())
                    {
                        foreach (var r in refs)
                        {
                            if (!LoadReference(r))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
                return false;
            }

        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="method">入口方法</param>
        /// <param name="runtime"></param>
        public static void Execute(MethodInfo method, dynamic runtime)
        {
            method.Invoke(null, new object[] { runtime });
        }

        /// <summary>
        /// 获取项目入口方法
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetProjectEntryMethodInfo()
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
                        if (Attribute.IsDefined(method, typeof(ProjectEntryMethod)))
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
                ScriptLogger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// 获取脚本入口方法
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetScriptEntryMethodInfo(string flieName)
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
                        if (Attribute.IsDefined(method, typeof(ScriptEntryMethod)))
                        {
                            if (method.IsStatic && method.IsPublic)
                            {
                                if (method.GetCustomAttribute<ScriptEntryMethod>().FileName == flieName)
                                {
                                    return method;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
            }

            return null;
        }



        public async Task<bool> Restore()
        {
            var projectPath = Directory.GetFiles(this.rootDir, "*.csproj", SearchOption.AllDirectories).First();
            if (projectPath is null)
            {
                return false;
            }

            try
            {
                TipsViewImpl.ChangeTipsText("正在还原包引用...");

                var xd = XDocument.Load(projectPath);
                var itemGroup = xd.Descendants("ItemGroup");

                var packageInfos = await RestorePackage(itemGroup);

                if (packageInfos is not null && packageInfos.Any())
                {
                    TipsViewImpl.ChangeTipsText("正在载入依赖项...");

                    if (!LoadPackageReference(packageInfos))
                    {
                        return false;
                    }
                }
                if (!LoadDllReference(itemGroup))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
                return false;
            }
        }

        private bool LoadPackageReference(List<PackageInfo> infos)
        {
            foreach (var info in infos)
            {
                foreach (var p in info.Paths)
                {
                    if (!LoadReference(p))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool LoadDllReference(IEnumerable<XElement> itemGroup)
        {
            var references = from element in itemGroup.Elements()
                             where element.Name == "Reference"
                             from child in element.Elements()
                             where child.Value.EndsWith(".dll")
                             select child.Value;

            foreach (var path in references)
            {
                if (!LoadReference(path))
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<List<PackageInfo>> RestorePackage(IEnumerable<XElement> itemGroup)
        {
            var restorePath = Path.Combine(this.rootDir, "obj", "project.packages.json");

            var packageReferenceItemGroups = from element in itemGroup.Elements()
                                             where element.Name == "PackageReference"
                                             select element.Attributes();

            var packageReferences = new Dictionary<string, NuGetVersion>();
            foreach (var group in packageReferenceItemGroups)
            {
                var pkgId = group.Where(x => x.Name == "Include").Select(x => x.Value).FirstOrDefault();
                var version = await NugetCommands.ParseVersion(pkgId,
                    group.Where(x => x.Name == "Version").Select(x => x.Value).FirstOrDefault() ?? "*");
                if (version is null)
                {
                    throw new Exception($"获取nuget包版本失败: \"{pkgId}\"");
                }
                packageReferences.Add(pkgId, version);
            }

            if (!File.Exists(restorePath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(restorePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(restorePath));
                }
                using var fs = File.Create(restorePath);
            }

            var storeInfos = JsonConvert.DeserializeObject<List<PackageInfo>>(File.ReadAllText(restorePath));

            var isNeedRestore = false;
            var notStorePackageReferences = new Dictionary<string, NuGetVersion>();

            foreach (var r in packageReferences)
            {
                notStorePackageReferences.Add(r.Key, r.Value);
            }

            if (storeInfos is not null)
            {
                foreach (var r in notStorePackageReferences)
                {
                    if (PackageInfo.Exists(storeInfos, r))
                    {
                        notStorePackageReferences.Remove(r.Key);
                    }
                }

                foreach (var info in storeInfos)
                {
                    foreach (var path in info.Paths)
                    {
                        if (!File.Exists(path))
                        {
                            isNeedRestore = true;
                            break;
                        }
                    }

                    if (isNeedRestore)
                    {
                        break;
                    }
                }

                if (packageReferences.Count != storeInfos.Count)
                {
                    isNeedRestore = true;
                }
            }

            if (notStorePackageReferences.Any() || isNeedRestore)
            {
                var transitiveDependences = await NugetCommands.ListPackageTransitiveDependenceAsync(packageReferences);
                storeInfos = await NugetCommands.GetPackageInfosAsync(transitiveDependences);
                File.WriteAllText(restorePath, JsonConvert.SerializeObject(storeInfos, Formatting.Indented));
            }

            return storeInfos;
        }


    }
}