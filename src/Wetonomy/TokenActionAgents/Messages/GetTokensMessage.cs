using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class GetTokensMessage<T>
    {
        public BigInteger Amount { get; }

        public T Recipient;

        public GetTokensMessage(T recipient, BigInteger amount)
        {
            Recipient = recipient;
            Amount = amount;
        }
    }
}
