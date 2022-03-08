using astator.Core.Engine;
using astator.Core.Script;
using astator.Core.ThirdParty;
using astator.NugetManager;
using astator.TipsView;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Xml.Linq;

namespace astator.Modules;
public class ApkBuilderer
{
    private readonly string rootDir;

    private readonly string outputDir;

    private readonly string assetsDir;

    private readonly string refDir;

    private readonly string csprojPath;

    private readonly string dllPath;

    private readonly string projectPath;

    public ApkBuilderer(string rootDir)
    {
        this.rootDir = rootDir;
        this.outputDir = Path.Combine(rootDir, "output");
        this.assetsDir = Path.Combine(rootDir, "assets");
        this.csprojPath = Directory.GetFiles(rootDir, "*.csproj", SearchOption.AllDirectories).First();
        this.dllPath = Path.Combine(this.outputDir, "compile.dll");
        this.projectPath = Path.Combine(this.outputDir, "project.zip");
        this.refDir = Path.Combine(this.outputDir, "ref");
    }


    public async Task<bool> Build()
    {
        TipsViewImpl.Show();

        return await Task.Run(async () =>
        {
            try
            {
                if (this.csprojPath is null)
                {
                    ScriptLogger.Error("*.csproj不存在!");
                    return false;
                }

                var xd = XDocument.Load(this.csprojPath);
                var config = xd.Descendants("ApkBuilderConfigs");

                var labelName = config.Select(x => x.Element("Label")).First()?.Value;
                var packageName = config.Select(x => x.Element("PackageName")).First()?.Value;
                var versionName = config.Select(x => x.Element("Version")).First()?.Value;
                if (string.IsNullOrEmpty(labelName)
                || string.IsNullOrEmpty(packageName)
                || string.IsNullOrEmpty(versionName))
                {
                    ScriptLogger.Error($"打包参数不完整! 应用名: {labelName}, 包名: {packageName}, 版本号: {versionName}");
                    return false;
                }

                var projectConfig = xd.Descendants("ProjectExtensions");
                var useOCR = Convert.ToBoolean(projectConfig.Select(x => x.Element("UseOCR")).First()?.Value);

                if (!await CompileDll(false))
                {
                    return false;
                }

                if (!ZipProject())
                {
                    return false;
                }

                var iconPath = Path.Combine(this.assetsDir, "appicon.png");

                if (!ApkBuilder.ApkBuilder.Build(this.outputDir, this.projectPath, versionName, packageName, labelName, iconPath, useOCR))
                {
                    return false;
                }

                ScriptLogger.Log($"打包apk成功, 保存路径: {Path.Combine(this.outputDir, $"{labelName}_{versionName}.apk")}");
                return true;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
                return false;
            }
            finally
            {
                TipsViewImpl.Hide();
                if (Directory.Exists(this.refDir)) Directory.Delete(this.refDir, true);
                if (File.Exists(this.dllPath)) File.Delete(this.dllPath);
                if (File.Exists(this.projectPath)) File.Delete(this.projectPath);
            }
        });
    }

    public bool ZipProject()
    {
        try
        {
            TipsViewImpl.ChangeTipsText("正在打包项目...");
            using var fs = new FileStream(this.projectPath, FileMode.Create);
            using var zip = new ZipArchive(fs, ZipArchiveMode.Update);

            zip.CreateEntryFromFile(this.dllPath, "compile.dll");

            if (Directory.Exists(this.assetsDir))
            {
                var files = Directory.GetFiles(this.assetsDir);
                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        var entryName = Path.GetRelativePath(this.rootDir, file);
                        zip.CreateEntryFromFile(file, entryName);
                    }
                }
            }

            if (Directory.Exists(this.refDir))
            {
                var refs = Directory.GetFiles(this.refDir);
                if (refs.Any())
                {
                    foreach (var r in refs)
                    {
                        var entryName = Path.GetRelativePath(this.outputDir, r);
                        zip.CreateEntryFromFile(r, entryName);
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



    public async Task<bool> CompileDll(bool createTipsView = true)
    {
        if (createTipsView) TipsViewImpl.Show();
        return await Task.Run(async () =>
        {
            try
            {
                if (string.IsNullOrEmpty(SdkReferences.SdkDir))
                {
                    await SdkReferences.Initialize();
                    if (string.IsNullOrEmpty(SdkReferences.SdkDir))
                    {
                        if (createTipsView) TipsViewImpl.Hide();
                        ScriptLogger.Error("获取sdk失败!");
                        return false;
                    }
                }

                var engine = new ScriptEngine(this.rootDir);
                if (!await engine.Restore())
                {
                    return false;
                }

                engine.ParseAllCS();
                var emitResult = engine.Compile(this.dllPath);

                if (!emitResult.Success)
                {
                    foreach (var item in emitResult.Diagnostics)
                    {
                        ScriptLogger.Error("编译失败: " + item.ToString());
                    }
                    return false;
                }

                var restorePath = Path.Combine(this.rootDir, "obj", "project.packages.json");
                var storeInfos = JsonConvert.DeserializeObject<List<PackageInfo>>(File.ReadAllText(restorePath));

                if (Directory.Exists(this.refDir))
                {
                    Directory.Delete(this.refDir, true);
                }

                Directory.CreateDirectory(this.refDir);

                var rootRefDir = Path.Combine(this.rootDir, "ref");
                if (Directory.Exists(rootRefDir))
                {
                    var refs = Directory.EnumerateFiles(rootRefDir, "*.dll", SearchOption.AllDirectories);
                    if (refs.Any())
                    {
                        foreach (var r in refs)
                        {
                            File.Copy(r, Path.Combine(this.refDir, Path.GetFileName(r)));
                        }
                    }
                }

                if (storeInfos is not null && storeInfos.Any())
                {
                    foreach (var info in storeInfos)
                    {
                        var paths = info.Paths;
                        foreach (var path in paths)
                        {
                            File.Copy(path, Path.Combine(this.refDir, Path.GetFileName(path)));
                        }
                    }
                }

                var xd = XDocument.Load(this.csprojPath);
                var config = xd.Descendants("ProjectExtensions");
                var isObfuscate = Convert.ToBoolean(config.Select(x => x.Element("IsObfuscate")).First()?.Value);

                if (isObfuscate)
                {
                    TipsViewImpl.ChangeTipsText("正在混淆dll...");
                    var rules = new ObfuscatorRules
                    {
                        DllPath = dllPath,
                        OutputDir = outputDir,
                        AssemblySearchPath = refDir
                    };
                    if (ObfuscatorHelper.Execute(rules))
                    {
                        return true;
                    }
                }

                if (createTipsView)
                {
                    Globals.Toast($"编译dll成功, 保存路径: {this.dllPath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
                return false;
            }
            finally
            {
                if (createTipsView) TipsViewImpl.Hide();
            }
        });
    }
}
