using Android.Util;

namespace astator.ApkBuilder.Axml.Chunks.Base;

internal class XmlAttribute
{
    public int NamespaceUri;
    public int Name;
    public int Value;
    public short StructureSize;
    public int Res0;
    public DataType Type;
    public int Data;

    public static XmlAttribute ReadInStream(Stream stream)
    {
        var namespaceUri = stream.ReadInt32();
        var name = stream.ReadInt32();
        var value = stream.ReadInt32();
        var structureSize = stream.ReadShort();
        var res0 = stream.ReadByte();
        var type = stream.ReadByte();
        var data = stream.ReadInt32();

        return new XmlAttribute
        {
            NamespaceUri = namespaceUri,
            Name = name,
            Value = value,
            StructureSize = structureSize,
            Res0 = res0,
            Type = (DataType)type,
            Data = data
        };
    }

    public XmlAttribute()
    {

    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream(5 * 4);
        stream.WriteInt32(this.NamespaceUri);
        stream.WriteInt32(this.Name);
        stream.WriteInt32(this.Value);
        stream.WriteShort(this.StructureSize);
        stream.WriteByte((byte)this.Res0);
        stream.WriteByte((byte)this.Type);
        stream.WriteInt32(this.Data);
        return stream.ToArray();
    }
}
