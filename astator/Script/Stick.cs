using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace astator.Script
{

    public struct NodeType
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public byte[] Data { get; set; }
    }

    public struct PackData
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("body")]
        public NodeType Body { get; set; }

        public byte[] ToBytes()
        {
            var result = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
            return result;
        }

        public override string ToString()
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }
    }

    public static class Stick
    {
        public static PackData MakePackData(string key, object body)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));

            var header = Int2Bytes(data.Length);

            var result = new PackData
            {
                Key = key,
                Body = new NodeType
                {
                    Type = "Buffer",
                    Data = header.Concat(data).ToArray()
                }
            };

            return result;
        }

        public static PackData MakePackData(string key, string body)
        {
            var data = Encoding.UTF8.GetBytes(body);

            var header = Int2Bytes(data.Length);

            var result = new PackData
            {
                Key = key,
                Body = new NodeType
                {
                    Type = "Buffer",
                    Data = header.Concat(data).ToArray()
                }
            };

            return result;
        }


        public static async Task<PackData> ReadPackAsync(Stream stream)
        {
            try
            {
                var header = new byte[4];

                var offset = 0;
                while (offset < 4)
                {
                    offset += await stream.ReadAsync(header.AsMemory(offset, 4 - offset));
                }

                var len = Bytes2Int(header);

                var data = new byte[len];


                offset = 0;
                while (offset < len)
                {
                    offset += await stream.ReadAsync(data.AsMemory(offset, len - offset));
                }

                var str = Encoding.UTF8.GetString(data);
                var result = JsonConvert.DeserializeObject<PackData>(Encoding.UTF8.GetString(data));

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }


        }



        private static byte[] Int2Bytes(long value)
        {
            var result = new byte[4];
            result[0] = (byte)((value >> 24) & 0xFF);
            result[1] = (byte)((value >> 16) & 0xFF);
            result[2] = (byte)((value >> 8) & 0xFF);
            result[3] = (byte)(value & 0xFF);
            return result;
        }

        public static int Bytes2Int(byte[] value, int offset = 0)
        {
            int result;
            result = ((value[offset] & 0xFF) << 24)
                    | ((value[offset + 1] & 0xFF) << 16)
                    | ((value[offset + 2] & 0xFF) << 8)
                    | (value[offset + 3] & 0xFF);
            return result;
        }

    }
}
