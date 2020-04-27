using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public class TransferTokenMessage
    {
        public BigInteger Amount { get; }
        public IAgentTokenKey From { get; }
        public IAgentTokenKey To { get; }

        public TransferTokenMessage(BigInteger amount, IAgentTokenKey from, IAgentTokenKey to)
        {
            Amount = amount;
            From = from;
            To = to;
        }
    }
}
