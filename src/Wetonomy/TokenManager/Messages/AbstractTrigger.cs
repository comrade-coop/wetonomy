using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    [System.Serializable]
    public abstract class AbstractTrigger
    {
        public BigInteger Amount { get; set; }

        public string Sender { get; protected set; }
    }
}
