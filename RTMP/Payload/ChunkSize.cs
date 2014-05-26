using System.IO;
using Mono;

namespace RTMPStreamReader.RTMP.Payload
{
    public class ChunkSize : IPayload
    {
        public static DataConverter dc = DataConverter.BigEndian;

        public int Size { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                Size = dc.GetInt32(ms.ReadBytes(4), 0) & 0x7FFFFFFF;
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(dc.GetBytes(Size & 0x7FFFFFFF), 0, 4);
                return ms.ToArray();
            }
        }
    }
}