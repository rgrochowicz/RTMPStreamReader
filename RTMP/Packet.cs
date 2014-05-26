using System;
using System.Collections.Generic;
using RTMPStreamReader.RTMP.Payload;

namespace RTMPStreamReader.RTMP
{
    public class Packet
    {
        public static Dictionary<PayloadType, Type> PayloadMapping;
        public byte[] BytePayload;

        public IPayload Payload;

        static Packet()
        {
            PayloadMapping = new Dictionary<PayloadType, Type>
            {
                {PayloadType.CHUNK_SIZE, typeof (ChunkSize)},
                {PayloadType.ABORT, typeof (Abort)},
                {PayloadType.BYTES_READ, typeof (BytesRead)},
                {PayloadType.CONTROL, typeof (Control)},
                {PayloadType.WINDOW_ACK_SIZE, typeof (WindowAckSize)},
                {PayloadType.SET_PEER_BW, typeof (SetPeerBandwidth)},
                {PayloadType.VIDEO, typeof (Video)},
                {PayloadType.AUDIO, typeof (Audio)},
                {PayloadType.METADATA_AMF0, typeof (MetadataAmf0)},
                {PayloadType.SHARED_OBJECT_AMF0, typeof(SharedObjectAmf0)},
                {PayloadType.INVOKE_AMF0, typeof (InvokeAmf0)},
                {PayloadType.AGGREGATE, typeof (Aggregate)}
            };
        }

        public Packet()
        {
            BytePayload = new byte[0];
            Reset();
        }

        public int ChunkType { get; set; }
        public int ChunkStreamId { get; set; }
        public int TimeStamp { get; set; }
        public int Length { get; set; }
        public PayloadType Type { get; set; }
        public int MessageStreamId { get; set; }
        public bool HasAbsTimestamp { get; set; }

        public int BytesRead { get; set; }

        public void Reset()
        {
            ChunkType = 0;
            ChunkStreamId = 0;
            TimeStamp = 0;
            Length = 0;
            Type = default(PayloadType);
            MessageStreamId = 0;
            HasAbsTimestamp = false;
            BytesRead = 0;
            BytePayload = new byte[0];
            Payload = default(IPayload);
        }

        public bool IsReady()
        {
            return BytesRead == Length;
        }

        public override string ToString()
        {
            return Type.ToString();
        }


        public void DecodePayload()
        {
            if (PayloadMapping.ContainsKey(Type))
            {
                Payload = (IPayload) Activator.CreateInstance(PayloadMapping[Type]);
                Payload.Decode(BytePayload);
            }
            else
            {
                throw new ArgumentException("Type not found in map");
            }
        }

        public void EncodePayload()
        {
            BytePayload = Payload.Encode();
            Length = BytePayload.Length;
        }
    }
}