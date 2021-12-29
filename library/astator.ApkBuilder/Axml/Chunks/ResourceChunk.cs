using astator.ApkBuilder.Axml.Chunks.Base;

namespace astator.ApkBuilder.Axml.Chunks;

internal class ResourceChunk : BaseChunk
{
    public List<int> ResourceIds = new();
    public ResourceChunk(Stream stream) : base(stream)
    {
        var count = (this.header.ChunkSize - 8) / 4;

        for (var i = 0; i < count; i++)
        {
            this.ResourceIds.Add(stream.ReadInt32());
        }
    }

    public override byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        stream.Write(this.header.ToBytes());
        foreach (var id in this.ResourceIds)
        {
            stream.WriteInt32(id);
        }
        return stream.ToArray();
    }
}
