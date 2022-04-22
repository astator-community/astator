
using astator.LoggerProvider;
using Java.Security;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace astator.ApkBuilder.Signer;

public class ApkSignerV1
{
    public static bool Sign(ZipArchive zip)
    {
        try
        {
            var (mfAttrs, mfBytes) = AddMF(zip);
            var sfBytes = AddSF(zip, mfAttrs, mfBytes);
            AddRSA(zip, sfBytes);
            return true;
        }
        catch (Exception ex)
        {
            AstatorLogger.Error(ex);
            return false;
        }
    }


    private static void AddRSA(ZipArchive zip, byte[] sfBytes)
    {
        var signature = GetSignature();
        signature.Update(sfBytes);
        var data = signature.Sign();

        var entry = zip.CreateEntry("META-INF/CERT.RSA");
        using var rsa = entry.Open();

        rsa.Write(Convert.FromBase64String(Util.SignPrefix));
        rsa.Write(data);
    }

    private static Signature GetSignature()
    {
        var signature = Signature.GetInstance("SHA1withRSA");
        var privateKey = Util.GetPrivateKey();
        signature.InitSign(privateKey);
        return signature;
    }

    private static byte[] AddSF(ZipArchive zip, Dictionary<string, string> mfAttrs, byte[] mfBytes)
    {
        var entry = zip.CreateEntry("META-INF/CERT.SF");
        using var sf = entry.Open();
        //using var writer = new StreamWriter(sf, Encoding.UTF8);

        using var sha256 = SHA1.Create();

        sf.Write(Encoding.UTF8.GetBytes("Signature-Version: 1.0\r\n"));
        sf.Write(Encoding.UTF8.GetBytes("X-Android-APK-Signed: 2\r\n"));
        sf.Write(Encoding.UTF8.GetBytes($"SHA1-Digest-Manifest: {Convert.ToBase64String(sha256.ComputeHash(mfBytes))}\r\n"));
        sf.Write(Encoding.UTF8.GetBytes("Created-By: astator.ApkBuilder\r\n"));
        sf.Write(Encoding.UTF8.GetBytes("\r\n"));


        foreach (var attr in mfAttrs)
        {
            var line = $"Name: {attr.Key}\r\n" +
               $"SHA1-Digest: {Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(attr.Value)))}" +
               $"\r\n\r\n";

            sf.Write(Encoding.UTF8.GetBytes(line));
        }

        var bytes = new byte[sf.Length];
        sf.Position = 0;
        sf.Read(bytes);
        return bytes;
    }

    private static (Dictionary<string, string> Attrs, byte[] Bytes) AddMF(ZipArchive zip)
    {
        var entries = zip.Entries.ToList();

        var attrs = new Dictionary<string, string>();

        var mfEntry = zip.CreateEntry("META-INF/MANIFEST.MF");
        using var mf = mfEntry.Open();

        mf.Write(Encoding.UTF8.GetBytes("Manifest-Version: 1.0\r\n"));
        mf.Write(Encoding.UTF8.GetBytes("Created-By: astator.ApkBuilder\r\n"));
        mf.Write(Encoding.UTF8.GetBytes("\r\n"));

        using var sha256 = SHA1.Create();

        foreach (var entry in entries)
        {
            using var stream = entry.Open();

            var line = $"Name: {entry.FullName}\r\n" +
           $"SHA1-Digest: {Convert.ToBase64String(sha256.ComputeHash(stream))}" +
           $"\r\n\r\n";

            mf.Write(Encoding.UTF8.GetBytes(line));

            attrs.Add(entry.FullName, line);

        }

        var bytes = new byte[mf.Length];
        mf.Position = 0;
        mf.Read(bytes);
        return (attrs, bytes);
    }
}
