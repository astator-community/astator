using astator.ApkBuilder.Axml.Chunks.Base;

namespace astator.ApkBuilder.Axml.Chunks;

internal class EndTagChunk : BaseContentChunk
{
    public int NameSpaceUri;
    public int Name;

    public EndTagChunk(Stream stream, StringChunk stringChunk) : base(stream, stringChunk)
    {
        this.NameSpaceUri = stream.ReadInt32();
        this.Name = stream.ReadInt32();

        stream.Position = this.startPosition + this.header.ChunkSize;
    }

    public override byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        stream.Write(this.header.ToBytes());
        stream.WriteInt32(this.lineNumber);
        stream.WriteInt32(this.Comment);
        stream.WriteInt32(this.NameSpaceUri);
        stream.WriteInt32(this.Name);

        return stream.ToArray();
    }

    public override string ToString()
    {
        return $"</{GetString(this.Name)}>{Environment.NewLine}";
    }
}
