namespace astator.ApkBuilder.Axml.Chunks.Base;

internal class BaseContentChunk : BaseChunk
{
    public int lineNumber;
    public int Comment;

    public StringChunk StringChunk;

    public BaseContentChunk(Stream stream, StringChunk stringChunk) : base(stream)
    {
        this.lineNumber = stream.ReadInt32();
        this.Comment = stream.ReadInt32();
        this.StringChunk = stringChunk;
    }

    public string GetString(int index)
    {
        return this.StringChunk.GetString(index);
    }

    public override byte[] ToBytes()
    {
        throw new NotImplementedException();
    }
}
