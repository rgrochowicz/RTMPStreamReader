namespace RTMPStreamReader.RTMP.Payload
{
    public enum PayloadType
    {
        CHUNK_SIZE = 0x01,
        ABORT = 0x02,
        BYTES_READ = 0x03,
        CONTROL = 0x04,
        WINDOW_ACK_SIZE = 0x05,
        SET_PEER_BW = 0x06,
        //0x07
        AUDIO = 0x08,
        VIDEO = 0x09,
        //0x0e
        METADATA_AMF3 = 0x0f,
        SHARED_OBJECT_AMF3 = 0x10,
        INVOKE_AMF3 = 0x11,
        METADATA_AMF0 = 0x12,
        SHARED_OBJECT_AMF0 = 0x13,
        INVOKE_AMF0 = 0x14,
        AGGREGATE = 0x16
    }
}