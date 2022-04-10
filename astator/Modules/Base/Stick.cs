namespace astator.Modules.Base;

public static class Stick
{
    public static byte[] MakePackData(string key, byte[] buffer)
    {
        var pack = new PackData
        {
            Key = key,
            Buffer = buffer
        };

        var data = pack.ToBytes();

        return data;
    }

    public static byte[] MakePackData(string key, string desc)
    {
        var pack = new PackData
        {
            Key = key,
            Description = desc
        };

        var data = pack.ToBytes();
        return data;
    }

    public static byte[] MakePackData(string key)
    {
        var pack = new PackData
        {
            Key = key
        };

        var data = pack.ToBytes();
        return data;
    }

    public static byte[] ToBytes(this int number)
    {
        var bytes = new byte[4];
        bytes[0] = (byte)(number >> 24);
        bytes[1] = (byte)(number >> 16);
        bytes[2] = (byte)(number >> 8);
        bytes[3] = (byte)number;
        return bytes;
    }

    public static int ToInt32(this byte[] value, int offset = 0)
    {
        int result;
        result = (value[offset] << 24)
                | (value[offset + 1] << 16)
                | (value[offset + 2] << 8)
                | (value[offset + 3]);
        return result;
    }

    public static void WriteInt32(this Stream stream, int number)
    {
        stream.Write(number.ToBytes());
    }

    public static int ReadInt32(this Stream stream)
    {
        var bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        return bytes.ToInt32();
    }

}
