using System.IO;

namespace RTMPStreamReader.RTMP.Payload.FLV
{
    public class VideoTag : Tag
    {
        public FrameType FrameType { get; set; }
        public Codec Codec { get; set; }
        public AvcType AVCType { get; set; }
        public uint CompositionTime { get; set; }

        public override TagType Type
        {
            get { return TagType.VIDEO; }
        }

        public override int PayloadSize
        {
            get
            {
                if (Codec == Codec.AVC)
                {
                    return Payload.Length + 1 + 1 + 3;
                }
                return Payload.Length + 1;
            }
        }


        public override void ReadContents(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var typeByte = (byte) ms.ReadByte();
                FrameType = (FrameType) (typeByte >> 4);
                Codec = (Codec) (typeByte & 0x0f);
                AVCType = AvcType.NotSet;
                if (Codec == Codec.AVC)
                {
                    AVCType = (AvcType) ms.ReadByte();
                    if (bytes.Length >= 5)
                    {
                        switch (AVCType)
                        {
                            case AvcType.Nalu:
                                CompositionTime = Utils.GetUInt24(ms.ReadBytes(3), 0);
                                break;
                            default:
                                CompositionTime = Utils.GetUInt24(ms.ReadBytes(3), 0);
                                break;
                        }
                    }
                }
                Payload = new byte[ms.Length - ms.Position];
                ms.Read(Payload, 0, Payload.Length);
            }
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);
            stream.WriteByte((byte) (((byte) FrameType << 4) | ((byte) Codec & 0x0f)));
            if (Codec == Codec.AVC)
            {
                stream.WriteByte((byte) AVCType);
                stream.Write(Utils.GetBytesUInt24(CompositionTime), 0, 3);
            }
            stream.Write(Payload, 0, Payload.Length);
        }
    }
}