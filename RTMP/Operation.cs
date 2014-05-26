using System;

namespace RTMPStreamReader.RTMP
{
    public class Operation
    {
        public Operation()
            : this(null, null)
        {
        }

        public Operation(Packet call, Action<Operation> handler)
        {
            if (call != null)
            {
                Call = call;
                Call.EncodePayload();
                ChunkStreamId = call.ChunkStreamId;
            }
            Handler = handler;
        }

        public int ChunkStreamId { get; set; }
        public Packet Call { get; set; }
        public Packet Response { get; set; }

        public Action<Operation> Handler { get; set; }

        public void CreateResponse(Packet packet)
        {
            Response = packet;
        }

        public void InvokeHandler()
        {
            if (Handler != null)
            {
                Handler(this);
            }
        }
    }
}