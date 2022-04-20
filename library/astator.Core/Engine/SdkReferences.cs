using astator.Core.Script;
using astator.NugetManager;
using astator.TipsView;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
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
                var version = await NugetCommands.ParseVersion(id, "0.2.*");
                var dir = Path.Combine(NugetCommands.NugetDirectory, id, version.ToString());
                if (!Directory.Exists(dir))
                {
                    TipsViewImpl.ChangeTipsText("正在下载sdk引用包...");
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
        if (!Android.App.Application.Context.PackageName.Equals(Globals.AstatorPackageName)) return;

        TipsViewImpl.ChangeTipsText("正在初始化sdk引用...");

        await CheckSdk();

        if (!string.IsNullOrEmpty(SdkDir))
        {
            var net6Dir = Path.Combine(SdkDir, "net6.0");
            var mauiDir = Path.Combine(SdkDir, "maui");

            foreach (var path in Directory.GetFiles(net6Dir, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    References.Add(MetadataReference.CreateFromFile(path));
                }
                catch { }
            }

            foreach (var path in Directory.GetFiles(mauiDir, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    References.Add(MetadataReference.CreateFromFile(path));
                }
                catch { }
            }
        }
    }
}
