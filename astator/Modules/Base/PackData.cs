using System.Text;

namespace astator.Modules.Base;
public class PackData
{
    public string Key { get; set; }

    public string Description { get; set; }

    public byte[] Buffer { get; set; }


    public byte[] ToBytes()
    {
        var size = 4 + 4 + 256 + 4 + 256 + 4 + (this.Buffer?.Length ?? 0);
        using var ms = new MemoryStream(size);
        ms.WriteInt32(size - 4);

        var keyBytes = Encoding.UTF8.GetBytes(this.Key);
        ms.WriteInt32(keyBytes.Length);
        ms.Write(keyBytes);

        if (this.Description is not null)
        {
            ms.Position = 4 + 4 + 256;
            var descBytes = Encoding.UTF8.GetBytes(this.Description);
            ms.WriteInt32(descBytes.Length);
            ms.Write(descBytes);
        }

        if (this.Buffer is not null)
        {
            ms.Position = 4 + 4 + 256 + 4 + 256;
            ms.WriteInt32(this.Buffer.Length);
            ms.Write(this.Buffer);
        }

        return ms.GetBuffer();
    }

    public static PackData Parse(byte[] bytes)
    {
        if (bytes is null) return null;

        try
        {
            var result = new PackData();
            using var ms = new MemoryStream(bytes);
            ms.Position = 4;
            var keySize = ms.ReadInt32();
            var keyBytes = new byte[keySize];
            ms.Read(keyBytes, 0, keySize);
            var key = Encoding.UTF8.GetString(keyBytes);
            result.Key = key;

            ms.Position = 4 + 4 + 256;
            var descSize = ms.ReadInt32();
            if (descSize > 0)
            {
                var descBytes = new byte[descSize];
                ms.Read(descBytes, 0, descSize);
                var desc = Encoding.UTF8.GetString(descBytes);
                result.Description = desc;
            }

            ms.Position = 4 + 4 + 256 + 4 + 256;
            var bufferSize = ms.ReadInt32();
            if (bufferSize > 0)
            {
                var buffer = new byte[bufferSize];
                ms.Read(buffer, 0, buffer.Length);
                result.Buffer = buffer;
            }

            return result;
        }
        catch
        {
            return null;
        }
    }
}
