using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.Controllers;
public struct PackData
{
    public string Key { get; set; }

    public string Description { get; set; }

    public byte[] Buffer { get; set; }


    public byte[] ToBytes()
    {
        var size = 4 + 4 + 256 + 4 + 256 + 4 + (this.Buffer?.Length ?? 0);
        var ms = new MemoryStream(size);
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
        var ms = new MemoryStream(bytes);

        var keySize = ms.ReadInt32();
        var keyBytes = new byte[keySize];
        ms.Read(keyBytes);
        var key = Encoding.UTF8.GetString(keyBytes);

        ms.Position = 4 + 256;
        var descSize = ms.ReadInt32();
        var descBytes = new byte[descSize];
        ms.Read(descBytes);
        var desc = Encoding.UTF8.GetString(descBytes);

        ms.Position = 4 + 256 + 4 + 256;
        var bufferSize = ms.ReadInt32();
        var buffer = new byte[bufferSize];
        ms.Read(buffer, 0, buffer.Length);

        return new PackData
        {
            Key = key,
            Description = desc,
            Buffer = buffer
        };

    }
}
