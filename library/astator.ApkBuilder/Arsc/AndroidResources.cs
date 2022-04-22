using astator.ApkBuilder.Axml.Chunks.Base;
using System.Text;

namespace astator.ApkBuilder.Arsc;

internal class AndroidResources
{
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
                var startPosition = stream.Position + 10;
                stream.Position = startPosition;
                var _bytes = new byte[256];
                stream.Write(_bytes);

                stream.Position = startPosition;
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
