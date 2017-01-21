using System;
using System.Numerics;

namespace Communicator.Common.Entities
{
    [Serializable]
    public class PublicKey
    {
        public BigInteger E { get; set; }

        public BigInteger N { get; set; }
    }
}
