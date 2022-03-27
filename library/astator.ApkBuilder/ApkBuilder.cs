using Android.Graphics;
using astator.ApkBuilder.Arsc;
using astator.ApkBuilder.Axml;
using astator.ApkBuilder.Signer;
using astator.TipsView;
using System.IO.Compression;
using Path = System.IO.Path;

namespace astator.ApkBuilder;

public class ApkBuilder
{
    private static readonly string templatePath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("template").ToString(), "template.apk");

    //注意: 在debug模式下打包的apk是无法打开的
    public static bool Build(string outputDir, string projectPath, string versionName, string packageName, string labelName, string iconPath, bool useOcr, bool isX86)
    {
        var apkPath = Path.Combine(outputDir, $"{labelName}_{versionName}{(isX86 ? "_x86" : string.Empty)}.apk");
        var alignedPath = Path.Combine(outputDir, $"{labelName}_{versionName}{(isX86 ? "_x86" : string.Empty)}-aligned.apk");

        try
        {
            CheckTemplateApk();
            if (!File.Exists(templatePath))
            {
                Console.WriteLine("模板apk不存在!");
                return false;
            }

            TipsViewImpl.ChangeTipsText("正在修改apk...");

            using var fs = new FileStream(apkPath, FileMode.Create);

            fs.Write(GetTemplateBytes());

            using var zip = new ZipArchive(fs, ZipArchiveMode.Update);

            zip.CreateEntryFromFile(projectPath, "assets/Resources/project.zip");

            var entries = zip.Entries.ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.Name == "AndroidManifest.xml")
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
                else if (entry.Name == "resources.arsc")
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
                else if (entry.Name == "appicon.png")
                {
                    if (File.Exists(iconPath))
                    {
                        var size = 72;
                        if (entry.FullName.StartsWith("res/mipmap-hdpi-v4")) size = 72;
                        else if (entry.FullName.StartsWith("res/mipmap-mdpi-v4")) size = 48;
                        else if (entry.FullName.StartsWith("res/mipmap-xhdpi-v4")) size = 96;
                        else if (entry.FullName.StartsWith("res/mipmap-xxhdpi-v4")) size = 144;
                        else if (entry.FullName.StartsWith("res/mipmap-xxxhdpi-v4")) size = 192;

                        var bitmap = BitmapFactory.DecodeFile(iconPath);
                        var newBitmap = Bitmap.CreateScaledBitmap(bitmap, size, size, true);
                        var bytes = newBitmap.AsImageBytes(Bitmap.CompressFormat.Png, 100);
                        bitmap.Recycle();
                        newBitmap.Recycle();

                        using var stream = entry.Open();
                        stream.Position = 0;
                        stream.Write(new byte[stream.Length]);
                        stream.Position = 0;
                        stream.Write(bytes);
                    }
                }
                else if (entry.Name == "appicon_background.png" || entry.Name == "appicon_foreground.png")
                {
                    if (File.Exists(iconPath))
                    {
                        var size = 162;
                        if (entry.FullName.StartsWith("res/mipmap-hdpi-v4")) size = 162;
                        else if (entry.FullName.StartsWith("res/mipmap-mdpi-v4")) size = 108;
                        else if (entry.FullName.StartsWith("res/mipmap-xhdpi-v4")) size = 216;
                        else if (entry.FullName.StartsWith("res/mipmap-xxhdpi-v4")) size = 324;
                        else if (entry.FullName.StartsWith("res/mipmap-xxxhdpi-v4")) size = 432;

                        var bitmap = BitmapFactory.DecodeFile(iconPath);
                        var newBitmap = Bitmap.CreateScaledBitmap(bitmap, size, size, true);
                        var bytes = newBitmap.AsImageBytes(Bitmap.CompressFormat.Png, 100);
                        bitmap.Recycle();
                        newBitmap.Recycle();

                        using var stream = entry.Open();
                        stream.Position = 0;
                        stream.Write(new byte[stream.Length]);
                        stream.Position = 0;
                        stream.Write(bytes);
                    }
                }
                else if (entry.Name.EndsWith(".RSA") || entry.Name.EndsWith(".SF") || entry.Name.EndsWith(".MF") ||
                         entry.Name == "libzipalign.so")
                {
                    entry.Delete();
                }
                else if (!isX86 && (entry.FullName.StartsWith("lib/x86")
                    || entry.FullName.StartsWith("lib/x86_64")
                    || entry.Name == "assemblies.x86.blob"
                    || entry.Name == "assemblies.x86_64.blob"))
                {
                    entry.Delete();
                }
                else if (isX86 && (entry.FullName.StartsWith("lib/armeabi-v7a")
                    || entry.FullName.StartsWith("lib/arm64-v8a")
                    || entry.Name == "assemblies.armeabi_v7a.blob"
                    || entry.Name == "assemblies.arm64_v8a.blob"))
                {
                    entry.Delete();
                }
                else if (!useOcr)
                {
                    if (entry.Name == "libc++_shared.so"
                        || entry.Name == "libhiai.so"
                        || entry.Name == "libhiai_ir.so"
                        || entry.Name == "libhiai_ir_build.so"
                        || entry.Name == "libNative.so"
                        || entry.Name == "libpaddle_light_api_shared.so")
                    {
                        entry.Delete();
                    }
                }
            }

            TipsViewImpl.ChangeTipsText("正在进行v1签名...");
            if (ApkSignerV1.Sign(zip))
            {
                zip.Dispose();
                fs.Dispose();

                TipsViewImpl.ChangeTipsText("正在进行zip对齐...");
                var aligned = Com.Mcal.Zipalign.Utils.ZipAligner.ZipAlign(apkPath, alignedPath);

                if (aligned)
                {
                    TipsViewImpl.ChangeTipsText("正在进行v2签名...");

                    var result = ApkSignerV2.Sign(alignedPath, apkPath);
                    if (result)
                    {
                        TipsViewImpl.ChangeTipsText($"打包apk成功, 保存路径: {apkPath}");
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
