using System.IO;

namespace RTMPStreamReader
{
    public static class ExtensionMethods
    {
        public static byte[] ReadBytes(this MemoryStream ms, int length)
        {
            var buffer = new byte[length];
            ms.Read(buffer, 0, length);
            return buffer;
        }
    }
}