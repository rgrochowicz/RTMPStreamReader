using RTMPStreamReader.RTMP.Payload.FLV;

namespace RTMPStreamReader.RTMP.Payload
{
    public class Audio : IMedia
    {
        public byte[] Bytes { get; set; }
        public Tag FlvTag { get; set; }

        public void Decode(byte[] bytes)
        {
            Bytes = bytes;
            FlvTag = new AudioTag();
            FlvTag.ReadContents(bytes);
        }

        public byte[] Encode()
        {
            return Bytes;
        }
    }
}