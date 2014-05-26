using System.Collections.Generic;
using System.IO;
using Amf;

namespace RTMPStreamReader.RTMP.Payload
{
    public class MetadataAmf0 : IPayload
    {
        public string Name { get; set; }
        public List<object> Data { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var ap = new AmfParser(ms);

                Name = (string) ap.ReadNextObject();
                Data = new List<object>();
                while (ms.Length != ms.Position)
                {
                    Data.Add(ap.ReadNextObject());
                }
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                var aw = new AmfWriter(ms);

                aw.Write(Name);
                Data.ForEach(aw.Write);

                return ms.ToArray();
            }
        }
    }
}