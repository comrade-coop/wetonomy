using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public abstract class AbstractTrigger
    {
        public BigInteger Amount { get; protected set; }

        public string Sender { get; protected set; }
    }
}
