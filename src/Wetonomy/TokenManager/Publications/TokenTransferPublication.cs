using System.Numerics;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenTransferPublication
    {
        public BigInteger Amount { get; }

        public IAgentTokenKey From { get; }

        public IAgentTokenKey To { get; }

        public TokenTransferPublication(BigInteger amount, IAgentTokenKey from, IAgentTokenKey to)
        {
            Amount = amount;
            From = from;
            To = to;
        }
    }
}