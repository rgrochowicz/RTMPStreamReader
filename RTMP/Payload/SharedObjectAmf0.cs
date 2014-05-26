namespace RTMPStreamReader.RTMP.Payload
{
    public class SharedObjectAmf0 : IPayload
    {
        private byte[] _bytes;

        public void Decode(byte[] bytes)
        {
            _bytes = bytes;
        }

        public byte[] Encode()
        {
            return _bytes;
        }
    }
}