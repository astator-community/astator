using astator.LoggerProvider;
using Java.Nio;
using Java.Security;
using Java.Security.Cert;
using ApkSignerV2Binding = Com.Android.Signapk.ApkSignerV2;

namespace astator.ApkBuilder.Signer;

internal class ApkSignerV2
{
    public static bool Sign(string alignedPath, string outputPath)
    {
        try
        {
            var privateKey = Util.GetPrivateKey();

            var certificate = ApkSignerV2Binding.GetX509Certificate(Util.Certificate);
            if (certificate is null)
            {
                return false;
            }


            var config = ApkSignerV2Binding.CreateV2SignerConfigs(new IPrivateKey[] { privateKey },
                new X509Certificate[] { certificate },
                new string[] { "SHA-256" });

            using var fs = new FileStream(alignedPath, FileMode.Open);
            var bytes = new byte[fs.Length];
            fs.Read(bytes);

            var buffers = ApkSignerV2Binding.Sign(ByteBuffer.Wrap(bytes), config);

            using var ofs = new FileStream(outputPath, FileMode.Create);

            foreach (var buffer in buffers)
            {
                buffer.Position(0);
                var _bytes = new byte[buffer.Limit()];
                buffer.Get(_bytes, 0, _bytes.Length);
                ofs.Write(_bytes);
            }

            return true;
        }
        catch (Exception ex)
        {
            AstatorLogger.Error(ex);
            return false;
        }
    }
}


