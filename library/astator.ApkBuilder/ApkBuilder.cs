using astator.ApkBuilder.Arsc;
using astator.ApkBuilder.Axml;
using astator.ApkBuilder.Signer;
using System.IO.Compression;

namespace astator.ApkBuilder;

public class ApkBuilder
{
    private static readonly string templatePath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("template").ToString(), "template.apk");

    //注意: 在debug模式下打包的apk是无法打开的
    public static bool Build(string outputDir, string projectPath, string versionName, string packageName, string labelName)
    {
        var apkPath = Path.Combine(outputDir, $"{labelName}_{versionName}.apk");
        var alignedPath = Path.Combine(outputDir, $"{labelName}_{versionName}-aligned.apk");

        try
        {
            CheckTemplateApk();
            if (!File.Exists(templatePath))
            {
                Console.WriteLine("模板apk不存在!");
                return false;
            }

            Console.WriteLine("正在修改apk...");

            using var fs = new FileStream(apkPath, FileMode.Create);

            fs.Write(GetTemplateBytes());

            using var zip = new ZipArchive(fs, ZipArchiveMode.Update);

            zip.CreateEntryFromFile(projectPath, "assets/Resources/project.zip");

            var entries = zip.Entries.ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.FullName == "AndroidManifest.xml")
                {
                    using var stream = entry.Open();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes);
                    stream.Position = 0;
                    stream.Write(new byte[bytes.Length]);
                    stream.Position = 0;

                    var axmlBytes = AndroidBinaryXml.Build(bytes, versionName, packageName, labelName);
                    stream.Write(axmlBytes);
                }
                else if (entry.FullName == "resources.arsc")
                {
                    using var stream = entry.Open();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes);
                    stream.Position = 0;
                    stream.Write(new byte[bytes.Length]);
                    stream.Position = 0;

                    var arscBytes = AndroidResources.Build(bytes, packageName);
                    stream.Write(arscBytes);
                }
                else if (entry.FullName.EndsWith(".RSA") || entry.FullName.EndsWith(".SF") || entry.FullName.EndsWith(".MF"))
                {
                    entry.Delete();
                }
            }

            Console.WriteLine("正在进行v1签名...");
            if (ApkSignerV1.Sign(zip))
            {
                zip.Dispose();
                fs.Dispose();

                Console.WriteLine("正在进行zip对齐...");
                var aligned = Com.Mcal.Zipalign.Utils.ZipAligner.ZipAlign(apkPath, alignedPath);

                if (aligned)
                {
                    Console.WriteLine("正在进行v2签名...");

                    var result = ApkSignerV2.Sign(alignedPath, apkPath);
                    if (result)
                    {
                        Console.WriteLine($"打包apk成功, 保存路径: {apkPath}");
                    }
                    return result;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            File.Delete(alignedPath);
        }
    }


    private static byte[] GetTemplateBytes()
    {
        using var fs = new FileStream(templatePath, FileMode.Open);
        var bytes = new byte[fs.Length];
        fs.Read(bytes);
        return bytes;
    }

    private static void CheckTemplateApk()
    {
        var basePath = Android.App.Application.Context.PackageManager.GetApplicationInfo(Android.App.Application.Context.PackageName, 0).SourceDir;
        if (File.Exists(basePath))
        {
            if (File.Exists(templatePath))
            {
                if (File.GetLastWriteTime(basePath) <= File.GetLastWriteTime(templatePath))
                {
                    return;
                }
            }
            File.Copy(basePath, templatePath, true);
        }
    }

}
