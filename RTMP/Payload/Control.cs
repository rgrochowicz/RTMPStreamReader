using System.IO;
using Mono;

namespace RTMPStreamReader.RTMP.Payload
{
    public class Control : IPayload
    {
        public static DataConverter dc = DataConverter.BigEndian;
        public ControlType Type { get; set; }
        public int StreamID { get; set; }
        public int BufferLength { get; set; }
        public int Time { get; set; }
        public byte[] Bytes { get; set; }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                Type = (ControlType) dc.GetInt16(ms.ReadBytes(2), 0);

                switch (Type)
                {
                    case ControlType.STREAM_BEGIN:
                    case ControlType.STREAM_EOF:
                    case ControlType.STREAM_DRY:
                    case ControlType.STREAM_IS_RECORDED:
                        StreamID = dc.GetInt32(ms.ReadBytes(4), 0);
                        break;
                    case ControlType.SET_BUFFER:
                        StreamID = dc.GetInt32(ms.ReadBytes(4), 0);
                        BufferLength = dc.GetInt32(ms.ReadBytes(4), 0);
                        break;
                    case ControlType.PING_REQUEST:
                    case ControlType.PING_RESPONSE:
                        Time = dc.GetInt32(ms.ReadBytes(4), 0);
                        break;
                    case ControlType.SWFV_REQUEST:
                        break;
                    case ControlType.SWFV_RESPONSE:
                        Bytes = ms.ReadBytes(42);
                        break;
                    case ControlType.BUFFER_FULL:
                    case ControlType.BUFFER_EMPTY:
                        StreamID = dc.GetInt32(ms.ReadBytes(4), 0);
                        break;
                }
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(dc.GetBytes((short) Type), 0, 2);
                switch (Type)
                {
                    case ControlType.STREAM_BEGIN:
                    case ControlType.STREAM_EOF:
                    case ControlType.STREAM_DRY:
                    case ControlType.STREAM_IS_RECORDED:
                        ms.Write(dc.GetBytes(StreamID), 0, 4);
                        break;
                    case ControlType.SET_BUFFER:
                        ms.Write(dc.GetBytes(StreamID), 0, 4);
                        ms.Write(dc.GetBytes(BufferLength), 0, 4);
                        break;
                    case ControlType.PING_REQUEST:
                    case ControlType.PING_RESPONSE:
                        ms.Write(dc.GetBytes(Time), 0, 4);
                        break;
                    case ControlType.SWFV_REQUEST:
                        break;
                    case ControlType.SWFV_RESPONSE:
                        ms.Write(Bytes, 0, 42);
                        break;
                    case ControlType.BUFFER_FULL:
                    case ControlType.BUFFER_EMPTY:
                        ms.Write(dc.GetBytes(StreamID), 0, 4);
                        break;
                }

                return ms.ToArray();
            }
        }
    }
}