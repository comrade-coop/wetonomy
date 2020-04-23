using System.Collections.Generic;
using System.Numerics;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenMintPublication
    {
        public BigInteger Amount { get; }

        public IAgentTokenKey To { get; }

        public TokenMintPublication(BigInteger amount, IAgentTokenKey to)
        {
            Amount = amount;
            To = to;
        }
    }
}