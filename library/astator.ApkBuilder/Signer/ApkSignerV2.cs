using Java.Nio;
using Java.Security;
using Java.Security.Cert;
using System.Text;
using ApkSignerV2Binding = Com.Android.Signapk.ApkSignerV2;

namespace astator.ApkBuilder.Signer;

internal class ApkSignerV2
{
    public static bool Sign(string alignedPath, string outputPath)
    {
        try
        {
            var privateKey = Util.GetPrivateKey();

            var config = ApkSignerV2Binding.CreateV2SignerConfigs(new IPrivateKey[] { privateKey }, new X509Certificate[] { ReadCertificate() }, new string[] { "SHA-256" });



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
            Console.WriteLine(ex.Message);
            return false;
        }
    }


    private static X509Certificate ReadCertificate()
    {
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(Util.Certificate));

        var cf = CertificateFactory.GetInstance("X.509");
        return (X509Certificate)cf.GenerateCertificate(ms);
    }
}
