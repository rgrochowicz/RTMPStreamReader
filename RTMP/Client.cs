using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RTMPStreamReader.RTMP.Payload;
using RTMPStreamReader.RTMP.Payload.FLV;
using File = RTMPStreamReader.RTMP.Payload.FLV.File;

namespace RTMPStreamReader.RTMP
{
    public class Client
    {
        private File _flv;
        private List<Packet> _packets;
        private StreamSocket _socket;
        private Stream _stream;
        private Thread _thread;
        private PacketWedge _wedge;
        private readonly Stream _flvStream;

        public Client(Stream flvStream)
        {
            _flvStream = flvStream;
            Stop = false;
        }

        public Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        public bool Stop { get; set; }

        public List<Packet> Packets
        {
            get { return _packets; }
            set { _packets = value; }
        }

        public event EventHandler OnPacketReceived;

        public void Connect(Stream stream)
        {
            _stream = stream;
            _thread = new Thread(_startThread)
            {
                Name = "Client Thread"
            };
            _thread.Start();
        }

        private void _startThread()
        {
            _socket = new StreamSocket(_stream);

            _socketHandshake();
            _socket.ResetCount();
            _wedge = new PacketWedge(_socket);
            _packetLoop();
        }

        private void _packetLoop()
        {
            _packets = new List<Packet>();
            _flv = new File(_flvStream);

            while (!Stop)
            {
                Packet packet;
                if ((packet = _wedge.Parse()) != null)
                {
                    //packets.Add(packet);
                    var media = packet.Payload as IMedia;
                    if (media != null)
                    {
                        IMedia im = media;

                        if (!(im.FlvTag.Type == TagType.VIDEO && im.FlvTag.PayloadSize <= 5) &&
                            !(im.FlvTag.Type == TagType.AUDIO && im.FlvTag.PayloadSize <= 1))
                        {
                            _flv.AddTag(im.FlvTag);
                        }
                    }
                    if (packet.Type == PayloadType.AGGREGATE)
                    {
                        var a = (Aggregate) packet.Payload;
                        long baseTimestamp = 0;
                        bool setBaseTimestamp = false;

                        foreach (AggregateMessage message in a.Messages)
                        {
                            if (!setBaseTimestamp)
                            {
                                setBaseTimestamp = true;
                                baseTimestamp = message.Timestamp;
                            }

                            message.Payload.FlvTag.TimeStamp =
                                (uint) (packet.TimeStamp + (int) message.Payload.FlvTag.TimeStamp - baseTimestamp);
                            _flv.AddTag(message.Payload.FlvTag);
                        }
                    }

                    if (_wedge.Operations.ContainsKey(packet.ChunkStreamId) &&
                        _wedge.Operations[packet.ChunkStreamId].Call == null)
                        _wedge.Operations.Remove(packet.ChunkStreamId);

                    if (OnPacketReceived != null)
                    {
                        OnPacketReceived(packet, new EventArgs());
                    }
                }
            }
        }


        private void _socketHandshake()
        {
            _socket.ReceiveBytes(1);
            _socket.ReceiveBytes(1536);
            _socket.ReceiveBytes(1536);
        }

        public void ForceStop()
        {
            _thread.Abort();
        }
    }
}