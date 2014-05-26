namespace RTMPStreamReader.RTMP.Payload
{
    public class AggregateMessage
    {
        public PayloadType Type { get; set; }
        public uint Length { get; set; }
        public uint Timestamp { get; set; }
        public uint StreamId { get; set; }
        public IMedia Payload { get; set; }
    }
}