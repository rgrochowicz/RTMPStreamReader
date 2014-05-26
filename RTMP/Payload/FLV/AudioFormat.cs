namespace RTMPStreamReader.RTMP.Payload.FLV
{
    public enum AudioFormat
    {
        LinearPCMPLatofrmEndian = 0,
        ADPCM = 1,
        MP3 = 2,
        LinearPCMLittleEndian = 3,
        Nellymoser_16kHzmono = 4,
        Nellymoser_8kHzmono = 5,
        Nellymoser = 6,
        G711ALawLogarithmicPCM = 7,
        G711MuLawLogarithmicPCM = 8,
        Reserved = 9,
        AAC = 10,
        Speex = 11,
        MP3_8kHz = 14,
        DeviceSpecificSound = 15
    }
}