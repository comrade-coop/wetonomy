using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public class BurnTokenMessage
    {
        public BigInteger Amount { get; }
        public IAgentTokenKey From { get; }

        public BurnTokenMessage(BigInteger amount, IAgentTokenKey from)
        {
            Amount = amount;
            From = from;
        }
    }
}
