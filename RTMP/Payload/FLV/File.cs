using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RTMPStreamReader.RTMP.Payload.FLV
{
    public class File
    {
        public List<Tag> Tags;
        private readonly Stream _stream;
        private bool _hasFirstPFrame;

        public File() : this(null)
        {
        }

        public File(Stream stream)
        {
            _stream = stream;
            _hasFirstPFrame = false;

            Tags = new List<Tag>();

            if (_stream == null)
                return;
            WriteHeader();
        }

        public void WriteHeader()
        {
            byte[] header =
            {
                Convert.ToByte('F'),
                Convert.ToByte('L'),
                Convert.ToByte('V'),
                0x01,
                0x05
            };
            header = header.Concat(Utils.Dc.GetBytes(9)).ToArray();
            _stream.Write(header, 0, header.Length);

            _stream.Write(Utils.Dc.GetBytes(0), 0, 4);
        }

        public void AddTag(Tag tag)
        {
            var videoTag = tag as VideoTag;
            if (videoTag != null && !_hasFirstPFrame)
            {
                if (videoTag.FrameType != FrameType.KeyFrame)
                    return;

                _hasFirstPFrame = true;
            }

            tag.Write(_stream);
            _stream.Write(Utils.Dc.GetBytes(tag.TotalSize), 0, 4);
        }
    }
}