namespace RTMPStreamReader.RTMP.Payload
{
    public enum ControlType
    {
        STREAM_BEGIN = 0,
        STREAM_EOF = 1,
        STREAM_DRY = 2,
        SET_BUFFER = 3,
        STREAM_IS_RECORDED = 4,
        PING_REQUEST = 6,
        PING_RESPONSE = 7,
        SWFV_REQUEST = 26,
        SWFV_RESPONSE = 27,
        BUFFER_EMPTY = 31,
        BUFFER_FULL = 32
    }
}