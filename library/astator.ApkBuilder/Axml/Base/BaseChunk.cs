namespace astator.ApkBuilder.Axml.Chunks.Base;
internal struct ChunkHeader
{
    public ChunkType ChunkType;
    public short HeaderSize;
    public int ChunkSize;

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream(8);
        stream.Write(((short)this.ChunkType).ToBytes());
        stream.Write(this.HeaderSize.ToBytes());
        stream.Write(this.ChunkSize.ToBytes());
        return stream.ToArray();
    }
}

public class BaseChunk
{
    internal ChunkHeader header;
    internal long startPosition;

    public BaseChunk(Stream stream)
    {
        stream.Position -= 2;
        this.startPosition = stream.Position;
        var type = (ChunkType)stream.ReadShort();
        var headerSize = stream.ReadShort();
        var ChunkSize = stream.ReadInt32();
        this.header = new ChunkHeader
        {
            ChunkType = type,
            HeaderSize = headerSize,
            ChunkSize = ChunkSize
        };
    }

    public virtual byte[] ToBytes()
    {
        throw new NotImplementedException();
    }
}
