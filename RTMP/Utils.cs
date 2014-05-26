using System;
using Mono;

namespace RTMPStreamReader.RTMP
{
    public class Utils
    {
        public static DataConverter Dc = DataConverter.BigEndian;


        public static unsafe uint GetUInt24(byte[] data, int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length - index < 3)
                throw new ArgumentException("index");
            if (index < 0)
                throw new ArgumentException("index");

            uint ret;
            var b = (byte*) &ret;

            for (int i = 0; i < 3; i++)
                b[2 - i] = data[index + i];

            return ret;
        }

        public static byte[] GetBytesUInt24(uint value)
        {
            var dest = new byte[] {0, 0, 0};

            dest[2] = (byte) (value & 0xff);
            dest[1] = (byte) ((value >> 8) & 0xff);
            dest[0] = (byte) ((value >> 16) & 0xff);

            return dest;
        }

    }
}