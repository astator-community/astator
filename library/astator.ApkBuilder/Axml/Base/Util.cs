namespace astator.ApkBuilder.Axml.Chunks.Base;

internal static class Util
{
    public static int ToInt32(this byte[] bytes)
    {
        return (bytes[3] << 24)
                | (bytes[2] << 16)
                | (bytes[1] << 8)
                | bytes[0];
    }

    public static byte[] ToBytes(this int number)
    {
        var bytes = new byte[4];
        bytes[0] = (byte)number;
        bytes[1] = (byte)(number >> 8);
        bytes[2] = (byte)(number >> 16);
        bytes[3] = (byte)(number >> 24);
        return bytes;
    }

    public static short ToShort(this byte[] bytes)
    {
        return (short)((bytes[1]) << 8
                | bytes[0]);
    }

    public static byte[] ToBytes(this short number)
    {
        var bytes = new byte[2];
        bytes[0] = (byte)(number & 0xFF);
        bytes[1] = (byte)((number >> 8) & 0xFF);
        return bytes;
    }


    public static short ReadShort(this Stream stream)
    {
        var bytes = new byte[2];
        stream.Read(bytes, 0, 2);
        return bytes.ToShort();
    }

    public static int ReadInt32(this Stream stream)
    {
        var bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        return bytes.ToInt32();
    }

    public static XmlAttribute ReadAttr(this Stream stream)
    {
        return XmlAttribute.ReadInStream(stream);
    }

    public static void WriteInt32(this Stream stream, int number)
    {
        stream.Write(number.ToBytes());
    }

    public static void WriteShort(this Stream stream, short number)
    {
        stream.Write(number.ToBytes());
    }

}
