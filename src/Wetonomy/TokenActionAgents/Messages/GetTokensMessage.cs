using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class GetTokensMessage
    {
        public BigInteger Amount { get; }

        public IAgentTokenKey Recipient;

        public GetTokensMessage(IAgentTokenKey recipient, BigInteger amount)
        {
            Recipient = recipient;
            Amount = amount;
        }
    }
}
