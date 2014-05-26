using System.IO;
using Mono;

namespace RTMPStreamReader.RTMP.Payload
{
    public class SetPeerBandwidth : IPayload
    {
        public static DataConverter dc = DataConverter.BigEndian;
        public int Value { get; set; }
        public LimitType Type { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                Value = dc.GetInt32(ms.ReadBytes(4), 0);
                Type = (LimitType) ms.ReadByte();
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(dc.GetBytes(Value), 0, 4);
                ms.WriteByte((byte) Type);
                return ms.ToArray();
            }
        }
    }
}