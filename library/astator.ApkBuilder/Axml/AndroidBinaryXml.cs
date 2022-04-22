
using astator.ApkBuilder.Axml.Chunks;
using astator.ApkBuilder.Axml.Chunks.Base;
using System.Text;

namespace astator.ApkBuilder.Axml;

internal class AndroidBinaryXml
{
    public static byte[] Build(byte[] bytes, string versionName, string packageName, string labelName)
    {
        using var stream = new MemoryStream();
        stream.Write(bytes);
        stream.Position = 0;

        var fileType = stream.ReadShort();
        var headerSize = stream.ReadShort();
        var fileSize = stream.ReadInt32();

        StringChunk stringChunk = null;
        ResourceChunk resourceChunk = null;

        var structs = new List<BaseContentChunk>();
        var namespaceChunks = new List<NamespaceChunk>();

        var versionCodeIndex = -1;
        var versionNameIndex = -1;
        var labelIndex = -1;

        var versionNameValueIndex = -1;

        var permissionIndex = -1;
        var manifestIndex = -1;
        var applicationIndex = -1;


        while (stream.Position < fileSize)
        {
            var type = (ChunkType)stream.ReadShort();

            if (type == ChunkType.String)
            {
                stringChunk = new StringChunk(stream, packageName);

                versionCodeIndex = stringChunk.GetIndex("versionCode");
                versionNameIndex = stringChunk.GetIndex("versionName");
                labelIndex = stringChunk.GetIndex("label");

                versionNameValueIndex = stringChunk.AddString(versionName);

                permissionIndex = stringChunk.GetIndex("uses-permission");
                manifestIndex = stringChunk.GetIndex("manifest");
                applicationIndex = stringChunk.GetIndex("application");
            }
            else if (type == ChunkType.Resource)
            {
                resourceChunk = new ResourceChunk(stream);
            }
            else if (type == ChunkType.StartNameSpace)
            {
                var chunk = new NamespaceChunk(stream, stringChunk);
                namespaceChunks.Add(chunk);
                structs.Add(chunk);
            }
            else if (type == ChunkType.StartTag)
            {
                var chunk = new StartTagChunk(stream, stringChunk, namespaceChunks);
                //if (chunk.Name != permissionIndex)
                //{
                if (chunk.Name == manifestIndex)
                {
                    foreach (var attr in chunk.Attributes)
                    {
                        if (attr.Name == versionNameIndex)
                        {
                            stringChunk.SetString(attr.Value, versionName);
                        }
                        else if (attr.Name == versionCodeIndex)
                        {
                            var ts = DateTime.Now - new DateTime(2022, 1, 1, 0, 0, 0, 0);
                            attr.Data = Convert.ToInt32(ts.TotalMinutes);
                        }
                    }
                }
                else if (chunk.Name == applicationIndex)
                {
                    foreach (var attr in chunk.Attributes)
                    {
                        if (attr.Name == labelIndex)
                        {
                            stringChunk.SetString(attr.Value, labelName);
                        }
                    }
                }
                structs.Add(chunk);
                //}
            }
            else if (type == ChunkType.EndTag)
            {
                var chunk = new EndTagChunk(stream, stringChunk);
                //if (chunk.Name != permissionIndex)
                //{
                structs.Add(chunk);
                //}
            }
            else if (type == ChunkType.EndNameSpace)
            {
                structs.Add(new NamespaceChunk(stream, stringChunk));
            }
        }
        var s = ToString(structs);

        return ToBytes(fileType, stringChunk, resourceChunk, structs);

    }


    public static string ToString(List<BaseContentChunk> structs)
    {
        var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>").Append(Environment.NewLine);

        var hasXmlns = false;

        foreach (var chunk in structs)
        {
            if (!hasXmlns && chunk is StartTagChunk startTagChunk)
            {
                startTagChunk.Addxmlns = true;
                hasXmlns = true;
            }
            sb.Append(chunk.ToString());
        }

        return sb.ToString();
    }

    private static byte[] ToBytes(short fileType, StringChunk stringChunk, ResourceChunk resourceChunk, List<BaseContentChunk> structs)
    {
        using var stream = new MemoryStream();
        stream.Position = 8;
        stream.Write(stringChunk.ToBytes());
        stream.Write(resourceChunk.ToBytes());

        var lineNumber = 1;
        foreach (var chunk in structs)
        {
            chunk.lineNumber = lineNumber;
            lineNumber++;

            stream.Write(chunk.ToBytes());
        }

        stream.Position = 0;
        stream.WriteShort(fileType);
        stream.WriteShort(8);
        stream.WriteInt32((int)stream.Length);

        return stream.ToArray();
    }
}
