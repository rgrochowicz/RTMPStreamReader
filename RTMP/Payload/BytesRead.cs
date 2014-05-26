using System.IO;
using Mono;

namespace RTMPStreamReader.RTMP.Payload
{
    public class BytesRead : IPayload
    {
        private static readonly DataConverter Dc = DataConverter.BigEndian;
        public int Sequence { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                Sequence = Dc.GetInt32(ms.ReadBytes(4), 0);
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(Dc.GetBytes(Sequence), 0, 4);
                return ms.ToArray();
            }
        }
    }
}