using RTMPStreamReader.RTMP.Payload.FLV;

namespace RTMPStreamReader.RTMP.Payload
{
    public class Video : IMedia
    {
        private byte[] Bytes { get; set; }
        public Tag FlvTag { get; set; }

        public void Decode(byte[] bytes)
        {
            Bytes = bytes;
            FlvTag = new VideoTag();
            FlvTag.ReadContents(bytes);
        }

        public byte[] Encode()
        {
            return Bytes;
        }
    }
}