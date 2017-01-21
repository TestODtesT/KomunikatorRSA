using System;
using System.Collections.Generic;
using System.Numerics;

namespace Communicator.Common.Entities
{
    [Serializable]
    public class Message
    {
        public MessageType Type { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; }

        public string UserName { get; set; }

        public string To { get; set; }

        public BigInteger[] EncryptedString { get; set; }

        public PublicKey PublicKey { get; set; }

        public Dictionary<string, PublicKey> ConnectedClients { get; set; }
    }
}
