using System.IO;

namespace RTMPStreamReader.RTMP.Payload.FLV
{
    public class AudioTag : Tag
    {
        private AudioFormat _audioFormat { get; set; }
        private AudioRate _audioRate { get; set; }
        private AudioSize _audioSize { get; set; }
        private AudioType _audioType { get; set; }
        private AACPacketType _aacPacketType { get; set; }
        private bool _emptyPayload { get; set; }


        public override TagType Type
        {
            get { return TagType.AUDIO; }
        }

        public override int PayloadSize
        {
            get
            {
                if (_emptyPayload) return 0;
                if (_audioFormat == AudioFormat.AAC)
                {
                    return Payload.Length + 2;
                }
                return Payload.Length + 1;
            }
        }

        public override void ReadContents(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                if (ms.Length == 0)
                {
                    _emptyPayload = true;
                    Payload = new byte[] {};
                }
                else
                {
                    _emptyPayload = false;

                    var audioByte = (byte) ms.ReadByte();

                    _audioFormat = (AudioFormat) (audioByte >> 4 & 0x0f);
                    _audioRate = (AudioRate) (audioByte >> 2 & 0x03);
                    _audioSize = (AudioSize) (audioByte >> 1 & 0x01);
                    _audioType = (AudioType) (audioByte & 0x01);

                    if (_audioFormat == AudioFormat.AAC)
                    {
                        if (_audioRate == AudioRate._44kH || _audioType == AudioType.Stereo)
                        {
                            _aacPacketType = (AACPacketType) ms.ReadByte();
                        }
                    }

                    Payload = new byte[ms.Length - ms.Position];
                    ms.Read(Payload, 0, Payload.Length);
                }
            }
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);
            if (_emptyPayload) return;

            int audioByte = ((byte) _audioFormat << 4) | ((byte) _audioRate << 2) | ((byte) _audioSize << 1) |
                            ((byte) _audioType & 0x01);

            stream.WriteByte((byte) audioByte);
            if (_audioFormat == AudioFormat.AAC)
            {
                stream.WriteByte((byte) _aacPacketType);
            }

            stream.Write(Payload, 0, Payload.Length);
        }
    }
}