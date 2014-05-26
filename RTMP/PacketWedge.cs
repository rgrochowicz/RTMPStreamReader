using System;
using System.Collections.Generic;
using System.IO;
using Mono;
using RTMPStreamReader.RTMP.Payload;

namespace RTMPStreamReader.RTMP
{
    public class PacketWedge
    {
        public const int MaxHeaderSize = 12;
        private readonly Dictionary<int, Packet> _previousReadPacket;
        private readonly StreamSocket _socket;

        private readonly Dictionary<int, int> _streamTimestamps;

        public int ChunkSizeR = 128;
        public int ChunkSizeW = 128;
        public int[] HeaderSizes = {12, 8, 4, 1};


        public PacketWedge(StreamSocket socket)
        {
            _socket = socket;
            _previousReadPacket = new Dictionary<int, Packet>();
            Operations = new Dictionary<int, Operation>();
            _streamTimestamps = new Dictionary<int, int>();
        }

        public Dictionary<int, Operation> Operations { get; set; }

        public Packet Parse()
        {
            var p = new Packet();

            byte header = _socket.ReceiveByte();

            int chunktype = (header & 0xC0) >> 6;
            p.ChunkType = chunktype;

            p.ChunkStreamId = header & 0x3F;

            switch (p.ChunkStreamId)
            {
                case 0:
                    p.ChunkStreamId = 64 + _socket.ReceiveByte();
                    break;
                case 1:
                    p.ChunkStreamId = 64 + _socket.ReceiveByte() + (_socket.ReceiveByte()*256);
                    break;
            }

            switch (p.ChunkType)
            {
                case 3:
                    p.TimeStamp = _previousReadPacket[p.ChunkStreamId].TimeStamp;
                    p.Length = _previousReadPacket[p.ChunkStreamId].Length;
                    p.Type = _previousReadPacket[p.ChunkStreamId].Type;
                    p.MessageStreamId = _previousReadPacket[p.ChunkStreamId].MessageStreamId;
                    break;
                case 2:
                    p.Length = _previousReadPacket[p.ChunkStreamId].Length;
                    p.Type = _previousReadPacket[p.ChunkStreamId].Type;
                    p.MessageStreamId = _previousReadPacket[p.ChunkStreamId].MessageStreamId;
                    break;
                case 1:
                    p.MessageStreamId = _previousReadPacket[p.ChunkStreamId].MessageStreamId;
                    break;
                case 0:
                    break;
            }

            _previousReadPacket[p.ChunkStreamId] = p;
            int headersize = HeaderSizes[p.ChunkType];

            if (headersize == MaxHeaderSize)
            {
                p.HasAbsTimestamp = true;
            }

            if (!Operations.ContainsKey(p.ChunkStreamId))
            {
                Operations[p.ChunkStreamId] = new Operation();
            }

            if (Operations[p.ChunkStreamId].Response != null)
            {
                p = Operations[p.ChunkStreamId].Response;
                headersize = 0;
            }
            else
            {
                Operations[p.ChunkStreamId].CreateResponse(p);
            }

            headersize--;
            var headerbytes = new byte[0];
            if (headersize > 0)
            {
                headerbytes = new byte[headersize];
                _socket.Receive(headerbytes, 0, headersize);
            }
            using (var headerstream = new MemoryStream(headerbytes))
            {
                if (headersize >= 3)
                {
                    p.TimeStamp = (int) Utils.GetUInt24(headerstream.ReadBytes(3), 0);
                    if (!p.HasAbsTimestamp)
                    {
                        if (!_streamTimestamps.ContainsKey(p.ChunkStreamId)) _streamTimestamps[p.ChunkStreamId] = 0;
                        p.TimeStamp += _streamTimestamps[p.ChunkStreamId];
                        _streamTimestamps[p.ChunkStreamId] = p.TimeStamp;
                    }
                }
                if (headersize >= 6)
                {
                    p.Length = (int) Utils.GetUInt24(headerstream.ReadBytes(3), 0);
                    p.BytesRead = 0;
                    p.BytePayload = null;
                }
                if (headersize > 6)
                {
                    p.Type = (PayloadType) headerstream.ReadByte();
                }
                if (headersize == 11)
                {
                    p.MessageStreamId = DataConverter.LittleEndian.GetInt32(headerstream.ReadBytes(4), 0);
                }
            }

            int ntoread = p.Length - p.BytesRead;
            int nchunk = ChunkSizeR;

            if (ntoread < nchunk)
            {
                nchunk = ntoread;
            }
            if (p.BytePayload == null)
            {
                p.BytePayload = new byte[0];
            }

            int prevLength = p.BytePayload.Length;
            Array.Resize(ref p.BytePayload, prevLength + nchunk);
            _socket.Receive(p.BytePayload, prevLength, nchunk);

            if (p.BytesRead + nchunk != p.BytePayload.Length)
            {
                throw new Exception(String.Format("Read failed, have read {0} of {1}", p.BytePayload.Length,
                    p.BytesRead + nchunk));
            }

            p.BytesRead += nchunk;

            if (!p.IsReady())
            {
                return null;
            }
            p.DecodePayload();

            //Special cases that affect the packet parser
            switch (p.Type)
            {
                case PayloadType.CHUNK_SIZE:
                    ChunkSizeR = ((ChunkSize) p.Payload).Size;
                    break;
                case PayloadType.VIDEO:
                    var v = (Video) p.Payload;
                    v.FlvTag.TimeStamp = (uint) p.TimeStamp;
                    //v.FlvTag.StreamID = (uint)p.MessageStreamID;
                    break;
                case PayloadType.AUDIO:
                    var a = (Audio) p.Payload;
                    a.FlvTag.TimeStamp = (uint) p.TimeStamp;
                    //v.FlvTag.StreamID = (uint)p.MessageStreamID;
                    break;
            }

            return p;
        }
    }
}