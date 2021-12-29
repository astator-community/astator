namespace astator.ApkBuilder.Axml.Chunks.Base;

internal enum ChunkType : short
{
    String = 0x0001,
    Resource = 0x0180,
    StartNameSpace = 0x0100,
    EndNameSpace = 0x0101,
    StartTag = 0x0102,
    EndTag = 0x0103,
    TablePackage = 0x200
}
