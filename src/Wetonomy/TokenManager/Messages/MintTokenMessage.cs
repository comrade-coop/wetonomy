using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public class MintTokenMessage
    {
        public BigInteger Amount { get; }
        public IAgentTokenKey To { get; }

        public MintTokenMessage(BigInteger amount, IAgentTokenKey to)
        {
            Amount = amount;
            To = to;
        }
    }
}
