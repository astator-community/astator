using astator.NugetManager;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace astator.Core.Engine;
public static class SdkReferences
{
    /// <summary>
    /// sdk路径
    /// </summary>
    public static string SdkDir { get; set; } = string.Empty;

    public static List<MetadataReference> References { get; private set; } = new();

    public static async Task<string> CheckSdk()
    {
        return await Task.Run(async () =>
        {
            if (string.IsNullOrEmpty(SdkDir))
            {
                var id = "astator.Sdk";
                var version = await NugetCommands.ParseVersion(id, "*");
                var dir = Path.Combine(NugetCommands.NugetDirectory, id, version.ToString());
                if (!Directory.Exists(dir))
                {
                    ScriptLogger.Log("正在下载sdk引用包...");
                    if (!await NugetCommands.DownLoadPackageAsync(id, version))
                    {
                        ScriptLogger.Error("下载sdk失败!");
                        return null;
                    }
                }
                SdkDir = Path.Combine(dir, "lib", "net6.0-android31.0");
            }

            return SdkDir;
        });
    }

    public static async Task Initialize()
    {
        ScriptLogger.Log("正在初始化sdk引用...");

        var assemblyNames = new List<string>();
        await CheckSdk();

        if (!string.IsNullOrEmpty(SdkDir))
        {
            foreach (var assembly in AssemblyLoadContext.Default.Assemblies)
            {
                assemblyNames.Add(assembly.GetName().Name);
            }

            var net6Dir = Path.Combine(SdkDir, "net6.0");
            var mauiDir = Path.Combine(SdkDir, "maui");

            foreach (var path in Directory.GetFiles(net6Dir, "*.dll", SearchOption.AllDirectories))
            {
                References.Add(MetadataReference.CreateFromFile(path));
                try
                {
                    var name = AssemblyName.GetAssemblyName(path).Name;
                    if (!assemblyNames.Contains(name))
                    {
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    }
                }
                catch { }

            }

            foreach (var path in Directory.GetFiles(mauiDir, "*.dll", SearchOption.AllDirectories))
            {
                References.Add(MetadataReference.CreateFromFile(path));
                try
                {
                    var name = AssemblyName.GetAssemblyName(path).Name;
                    if (!assemblyNames.Contains(name))
                    {
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    }
                }
                catch { }
            }
        }

    }
}
