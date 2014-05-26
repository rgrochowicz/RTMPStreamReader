using System.IO;

namespace RTMPStreamReader.RTMP
{
    public class StreamSocket
    {
        private readonly Stream _stream;

        public StreamSocket(Stream stream)
        {
            Count = 0;
            _stream = stream;
        }

        public int Available
        {
            get { return (int) (_stream.Length - _stream.Position); }
        }

        public int Count { get; set; }

        public int Receive(byte[] buffer, int offset, int size)
        {
            Count += size;
            return _stream.Read(buffer, offset, size);
        }

        public int Receive(byte[] buffer)
        {
            Count += buffer.Length;
            return _stream.Read(buffer, 0, buffer.Length);
        }

        public byte[] ReceiveBytes(int bytecount)
        {
            Count += bytecount;
            var buffer = new byte[bytecount];
            _stream.Read(buffer, 0, bytecount);
            return buffer;
        }

        public byte ReceiveByte()
        {
            Count += 1;
            return (byte) _stream.ReadByte();
        }

        public void ResetCount()
        {
            Count = 0;
        }
    }
}