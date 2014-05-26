using System.Collections.Generic;
using System.IO;

namespace RTMPStreamReader.RTMP.Payload
{
    public class Aggregate : IPayload
    {
        public List<AggregateMessage> Messages { get; set; }

        public void Decode(byte[] bytes)
        {
            Messages = new List<AggregateMessage>();

            using (var ms = new MemoryStream(bytes))
            {
                while (ms.Position < ms.Length)
                {
                    var pt = (PayloadType) ms.ReadByte();

                    uint length = Utils.GetUInt24(ms.ReadBytes(3), 0);

                    var timestamp =
                        (uint) (ms.ReadByte() << 16 | ms.ReadByte() << 8 | ms.ReadByte() | ms.ReadByte() << 24);

                    uint streamid = Utils.GetUInt24(ms.ReadBytes(3), 0);

                    var payloadbytes = new byte[(int) length];
                    ms.Read(payloadbytes, 0, (int) length);

                    //backpointer
                    Utils.Dc.GetUInt32(ms.ReadBytes(4), 0);

                    IMedia payload;
                    switch (pt)
                    {
                        case PayloadType.AUDIO:
                            payload = new Audio();
                            break;
                        case PayloadType.VIDEO:
                            payload = new Video();
                            break;
                        default:
                            continue;
                    }
                    payload.Decode(payloadbytes);

                    payload.FlvTag.StreamId = 0;
                    payload.FlvTag.TimeStamp = timestamp;
                    var am = new AggregateMessage
                    {
                        Type = pt,
                        Length = length,
                        Timestamp = timestamp,
                        StreamId = streamid,
                        Payload = payload
                    };
                    Messages.Add(am);
                }
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                return ms.ToArray();
            }
        }
    }
}