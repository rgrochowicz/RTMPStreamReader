using System.IO;
using Mono;

namespace RTMPStreamReader.RTMP.Payload
{
    public class Abort : IPayload
    {
        public static DataConverter dc = DataConverter.BigEndian;
        public int StreamId { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                StreamId = dc.GetInt32(ms.ReadBytes(4), 0);
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(dc.GetBytes(StreamId), 0, 4);
                return ms.ToArray();
            }
        }
    }
}