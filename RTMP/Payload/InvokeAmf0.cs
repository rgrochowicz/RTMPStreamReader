using System.Collections.Generic;
using System.IO;
using Amf;

namespace RTMPStreamReader.RTMP.Payload
{
    public class InvokeAmf0 : IPayload
    {
        public static int CurrentTransactionID = 2;

        public InvokeAmf0()
        {
            AmfVersion = 0;
            Arguments = new List<object>();
            TransactionID = -2;
        }

        public string CommandName { get; set; }

        /// <summary>
        ///     -2 = Auto-assign
        ///     -1 = Zero
        ///     Any other = That value
        /// </summary>
        public int TransactionID { get; set; }

        public AmfObject CommandObject { get; set; }
        public List<object> Arguments { get; set; }
        public int AmfVersion { get; set; }

        public bool IsError
        {
            get { return CommandName == "_error"; }
        }

        public void Decode(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var ap = new AmfParser(ms);
                CommandName = (string) ap.ReadNextObject();
                TransactionID = (int) ((double) ap.ReadNextObject());
                CommandObject = (AmfObject) ap.ReadNextObject();

                while (ms.Length != ms.Position)
                {
                    Arguments.Add(ap.ReadNextObject());
                }
            }
        }

        public byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                var aw = new AmfWriter(ms);

                aw.Write(CommandName);

                int transid = 0;
                switch (transid)
                {
                    case -2:
                        transid = CurrentTransactionID++;
                        break;
                    case -1:
                        transid = 0;
                        break;
                    default:
                        transid = TransactionID;
                        break;
                }
                aw.Write(transid);

                aw.Write(CommandObject);

                if (Arguments != null)
                {
                    Arguments.ForEach(aw.Write);
                }

                return ms.ToArray();
            }
        }

        public bool IsResponseCommand()
        {
            return CommandName == "_result" || CommandName == "_error";
        }
    }
}