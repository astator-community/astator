using Android.Util;
using astator.ApkBuilder.Axml.Chunks.Base;
using System.Text;

namespace astator.ApkBuilder.Axml.Chunks;

internal class StartTagChunk : BaseContentChunk
{
    public int NameSpaceUri;
    public int Name;
    public short AttributeStart;
    public short AttributeSize;
    public short AttributeCount;
    public short IdIndex;
    public short ClassIndex;
    public short StyleIndex;

    public List<XmlAttribute> Attributes;

    public List<NamespaceChunk> NamespaceChunkList;

    public bool Addxmlns = false;

    public StartTagChunk(Stream stream, StringChunk stringChunk, List<NamespaceChunk> namespaceChunkList) : base(stream, stringChunk)
    {
        this.NameSpaceUri = stream.ReadInt32();
        this.Name = stream.ReadInt32();
        this.AttributeStart = stream.ReadShort();
        this.AttributeSize = stream.ReadShort();
        this.AttributeCount = stream.ReadShort();
        this.IdIndex = stream.ReadShort();
        this.ClassIndex = stream.ReadShort();
        this.StyleIndex = stream.ReadShort();

        this.Attributes = new(this.AttributeCount);
        for (var i = 0; i < this.AttributeCount; i++)
        {
            this.Attributes.Add(stream.ReadAttr());
        }

        this.NamespaceChunkList = namespaceChunkList;
        stream.Position = this.startPosition + this.header.ChunkSize;
    }

    public override byte[] ToBytes()
    {
        using var stream = new MemoryStream(this.header.ChunkSize);
        this.AttributeCount = (short)this.Attributes.Count;
        stream.Write(this.header.ToBytes());
        stream.WriteInt32(this.lineNumber);
        stream.WriteInt32(this.Comment);
        stream.WriteInt32(this.NameSpaceUri);
        stream.WriteInt32(this.Name);
        stream.WriteShort(this.AttributeStart);
        stream.WriteShort(this.AttributeSize);
        stream.WriteShort(this.AttributeCount);
        stream.WriteShort(this.IdIndex);
        stream.WriteShort(this.ClassIndex);
        stream.WriteShort(this.StyleIndex);

        foreach (var attr in this.Attributes)
        {
            stream.Write(attr.ToBytes());
        }


        return stream.ToArray();
    }

    public string GetPrefix(int uri)
    {
        if (this.NamespaceChunkList is not null)
        {
            foreach (var chunk in this.NamespaceChunkList)
            {
                if (chunk.Uri == uri)
                {
                    return GetString(chunk.Prefix);
                }
            }
        }
        return null;
    }

    public string GetNameSpace()
    {
        var result = string.Empty;
        if (this.NamespaceChunkList is not null)
        {
            foreach (var chunk in this.NamespaceChunkList)
            {
                result += $" {chunk.GetXmlNameSpace()}";
            }
        }
        return result;
    }

    public override string ToString()
    {
        var tagBuilder = new StringBuilder();

        if (this.Comment > -1)
        {
            tagBuilder.Append("<!--").Append(GetString(this.Comment)).Append("-->").Append(Environment.NewLine);
        }

        tagBuilder.Append('<');

        var tagName = GetString(this.Name);
        tagBuilder.Append(tagName);

        if (this.Addxmlns)
        {
            tagBuilder.Append(GetNameSpace());
        }

        foreach (var attr in this.Attributes)
        {
            tagBuilder.Append(' ');
            if (attr.NamespaceUri != -1)
            {
                tagBuilder.Append(GetPrefix(attr.NamespaceUri)).Append(':');
            }

            var data = TypedValue.CoerceToString(attr.Type, attr.Data) ?? GetString(attr.Value);
            tagBuilder.Append(GetString(attr.Name))
                .Append('=')
                .Append('"')
                .Append(data)
                .Append('"');
        }
        return tagBuilder.Append('>').Append(Environment.NewLine).ToString();
    }


}

