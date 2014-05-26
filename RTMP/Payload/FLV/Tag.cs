using System;
using System.IO;

namespace RTMPStreamReader.RTMP.Payload.FLV
{
    public abstract class Tag
    {
        public bool Filter { get; set; }
        public abstract TagType Type { get; }
        public uint DataSize { get; set; }
        public uint TimeStamp { get; set; }
        public uint StreamId { get; set; }

        public byte[] Payload { get; set; }

        public int TotalSize
        {
            get { return PayloadSize + 11; }
        }

        public abstract int PayloadSize { get; }

        public abstract void ReadContents(byte[] bytes);


        /*public static Tag Parse(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Tag t = new Tag();
                byte headerbyte = (byte)ms.ReadByte();
                t.Filter = ((headerbyte >> 5) & 0x01) == 0x01;
                t.Type = (TagType)(headerbyte & 0x1f);
                t.DataSize = Utils.GetUInt24(ms.ReadBytes(3), 0);
                t.TimeStamp = Utils.GetUInt24(ms.ReadBytes(3), 0);
                ms.ReadByte();
                t.StreamID = Utils.GetUInt24(ms.ReadBytes(3), 0);
                return t;
                
            }
        }*/

        public virtual void Write(Stream stream)
        {
            stream.WriteByte((byte) (Convert.ToByte(Filter) << 4 | (byte) Type & 0x1f));
            stream.Write(Utils.GetBytesUInt24((uint) PayloadSize), 0, 3);
            stream.Write(Utils.GetBytesUInt24(TimeStamp), 0, 3);
            //Debug.Print("FLV TAG TS: {0}, {1}", this.TimeStamp, String.Join(", ", tsbytes));
            stream.WriteByte(0);
            stream.Write(Utils.GetBytesUInt24(StreamId), 0, 3);
        }
    }
}