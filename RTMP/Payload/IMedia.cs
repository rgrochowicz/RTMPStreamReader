using RTMPStreamReader.RTMP.Payload.FLV;

namespace RTMPStreamReader.RTMP.Payload
{
    public interface IMedia : IPayload
    {
        Tag FlvTag { get; set; }
    }
}