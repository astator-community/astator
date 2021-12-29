using astator.ApkBuilder.Axml.Chunks.Base;

namespace astator.ApkBuilder.Axml.Chunks;

internal class NamespaceChunk : BaseContentChunk
{
    public int Prefix;
    public int Uri;

    public NamespaceChunk(Stream stream, StringChunk stringChunk) : base(stream, stringChunk)
    {
        this.Prefix = stream.ReadInt32();
        this.Uri = stream.ReadInt32();
        stream.Position = this.startPosition + this.header.ChunkSize;
    }

    public string GetXmlNameSpace()
    {
        return $"xmlns:{GetString(this.Prefix)}=\"{GetString(this.Uri)}\"";
    }

    public override string ToString()
    {
        return string.Empty;
    }

    public override byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        stream.Write(this.header.ToBytes());
        stream.WriteInt32(this.lineNumber);
        stream.WriteInt32(this.Comment);
        stream.WriteInt32(this.Prefix);
        stream.WriteInt32(this.Uri);
        return stream.ToArray();
    }
}
