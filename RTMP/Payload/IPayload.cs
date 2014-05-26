namespace RTMPStreamReader.RTMP.Payload
{
    public interface IPayload
    {
        void Decode(byte[] bytes);
        byte[] Encode();
    }
}