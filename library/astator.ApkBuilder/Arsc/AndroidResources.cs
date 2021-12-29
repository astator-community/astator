using astator.ApkBuilder.Axml.Chunks.Base;
using System.Text;

namespace astator.ApkBuilder.Arsc;

internal class AndroidResources
{
    public static readonly string path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("template").ToString(), "resources.arsc");
    public static byte[] Build(byte[] bytes, string packageName)
    {
        using var stream = new MemoryStream();
        stream.Write(bytes);
        stream.Position = 12;

        while (stream.Position < stream.Length)
        {
            var type = (ChunkType)stream.ReadShort();

            if (type == ChunkType.TablePackage)
            {
                var startposition = stream.Position + 10;
                stream.Position = startposition;
                var _bytes = new byte[256];
                stream.Write(_bytes);

                stream.Position = startposition;
                _bytes = Encoding.Unicode.GetBytes(packageName);

                stream.Write(_bytes);
                break;
            }
            else
            {
                var chunk = new BaseChunk(stream);
                stream.Position = chunk.startPosition + chunk.header.ChunkSize;
            }
        }

        return stream.ToArray();

    }
}
