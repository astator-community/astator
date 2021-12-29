using astator.ApkBuilder.Arsc;
using astator.ApkBuilder.Axml;
using astator.ApkBuilder.Signer;
using System.IO.Compression;

namespace astator.ApkBuilder;

public class ApkBuilder
{
    private static readonly string templatePath = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("tempalte").ToString(), "tempalte.apk");

    public static bool Build(string outputPath, string injectDir, string versionName, string packageName, string labelName)
    {
        var alignedPath = outputPath[0..^4] + "-aligned.apk";
        try
        {
            if (!File.Exists(templatePath))
            {
#if DEBUG
                CopyTemplate();
#else
                Console.WriteLine($"{templatePath}不存在!");
                return false;
#endif
            }

            using var fs = new FileStream(outputPath, FileMode.Create);

            fs.Write(GetTemplateBytes());

            using var zip = new ZipArchive(fs, ZipArchiveMode.Update);

            var entries = zip.Entries;

            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.FullName == "AndroidManifest.xml")
                {
                    using var stream = entry.Open();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes);
                    stream.Flush();
                    var axmlBytes = AndroidBinaryXml.Build(bytes, versionName, packageName, labelName);
                    stream.Position = 0;
                    stream.Write(axmlBytes);
                }
                else if (entry.FullName == "resources.arsc")
                {
                    using var stream = entry.Open();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes);
                    stream.Flush();
                    var arscBytes = AndroidResources.Build(bytes, packageName);
                    stream.Position = 0;
                    stream.Write(arscBytes);
                }
            }

            if (ApkSignerV1.Sign(zip))
            {
                zip.Dispose();
                fs.Dispose();

                var aligned = Com.Mcal.Zipalign.Utils.ZipAligner.ZipAlign(outputPath, alignedPath);

                return ApkSignerV2.Sign(alignedPath, outputPath);
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
        fs.Position = 0;
        fs.Read(bytes);
        return bytes;
    }

    private static async void CopyTemplate()
    {
        using var fs = new FileStream(templatePath, FileMode.Create);
        using var source = await FileSystem.OpenAppPackageFileAsync("Resources/template.apk");
        var bytes = new byte[1024];

        var len = 0;
        while ((len = source.Read(bytes)) > 0)
        {
            fs.Write(bytes, 0, len);
        }
    }

}
