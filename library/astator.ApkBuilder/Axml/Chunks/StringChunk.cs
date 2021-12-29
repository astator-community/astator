using astator.ApkBuilder.Axml.Chunks.Base;
using System.Text;

namespace astator.ApkBuilder.Axml.Chunks;

internal class StringChunk : BaseChunk
{
    public int StringCount;
    public int StyleCount;
    public bool IsUTF8;
    public bool IsSorted;
    public int StringStartPosition;
    public int StyleStartPosition;
    public int[] StringOffsets;
    public int[] StyleOffsets;
    public List<string> Strings = new List<string>();

    public StringChunk(Stream stream, string destPackageName) : base(stream)
    {
        this.StringCount = stream.ReadInt32();
        this.StyleCount = stream.ReadInt32();
        this.IsUTF8 = stream.ReadShort() != 0;
        this.IsSorted = stream.ReadShort() != 0;
        this.StringStartPosition = stream.ReadInt32();
        this.StyleStartPosition = stream.ReadInt32();


        this.StringOffsets = new int[this.StringCount];
        for (var i = 0; i < this.StringCount; i++)
        {
            this.StringOffsets[i] = stream.ReadInt32();
        }

        this.StyleOffsets = new int[this.StyleCount];
        for (var i = 0; i < this.StyleCount; i++)
        {
            this.StyleOffsets[i] = stream.ReadInt32();
        }

        //var packageName = Android.App.Application.Context.ApplicationContext.PackageName;

        var packageName = "com.companyname.astator";

        for (var i = 0; i < this.StringCount; i++)
        {
            stream.Position = this.startPosition + this.StringStartPosition + this.StringOffsets[i];
            var byteCount = this.IsUTF8 ? stream.ReadByte() : stream.ReadShort() * 2;
            var strBytes = new byte[byteCount];
            stream.Read(strBytes, 0, byteCount);

            var str = this.IsUTF8 ? Encoding.UTF8.GetString(strBytes) : Encoding.Unicode.GetString(strBytes);
            if (str.IndexOf(packageName) != -1)
            {
                str = str.Replace(packageName, destPackageName);
            }
            this.Strings.Add(str);
        }

        stream.Position = this.startPosition + this.header.ChunkSize;
    }

    public override byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        this.StringCount = this.Strings.Count;
        stream.Position = 8;
        stream.WriteInt32(this.StringCount);
        stream.WriteInt32(this.StyleCount);
        stream.Write(new byte[] { 0, (byte)(this.IsUTF8 ? 1 : 0) });
        stream.Write(new byte[] { 0, (byte)(this.IsSorted ? 1 : 0) });
        stream.WriteInt32(this.StringStartPosition);
        stream.WriteInt32(this.StyleStartPosition);

        var stringOffset = 0;

        foreach (var str in this.Strings)
        {
            stream.WriteInt32(stringOffset);
            Console.WriteLine(stringOffset);
            if (this.IsUTF8)
            {
                stringOffset += str.Length + 3;
            }
            else
            {
                stringOffset += str.Length * 2 + 4;
            }
        }

        var stringStartPosition = (int)stream.Position;

        foreach (var str in this.Strings)
        {
            if (this.IsUTF8)
            {
                var strBytes = Encoding.UTF8.GetBytes(str);
                stream.WriteByte((byte)str.Length);
                stream.WriteShort((byte)strBytes.Length);
                stream.Write(strBytes);
                stream.WriteByte(0);
            }
            else
            {
                var strBytes = Encoding.Unicode.GetBytes(str);
                stream.WriteShort((short)str.Length);
                stream.Write(strBytes);
                stream.WriteShort(0);
            }
        }
        if (stream.Length % 4 != 0)
        {
            var offset = 4 - (stream.Length % 4);
            stream.Write(new byte[offset]);
        }

        stream.Position = 20;
        stream.WriteInt32(stringStartPosition);

        this.header.ChunkSize = (int)stream.Length;
        stream.Position = 0;
        stream.Write(this.header.ToBytes());
        return stream.ToArray();
    }

    public int AddString(string str)
    {
        this.Strings.Add(str);
        this.StringCount++;
        return GetIndex(str);
    }

    public int GetIndex(string value)
    {
        return this.Strings.FindIndex(str => str == value);
    }

    public string GetString(int index)
    {
        if (index > 0 && index < this.Strings.Count)
        {
            return this.Strings[index];
        }
        return null;
    }

    public void SetString(int index, string value)
    {
        if (index > 0 && index < this.Strings.Count)
        {
            this.Strings[index] = value;
        }
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, this.Strings);
    }


}
