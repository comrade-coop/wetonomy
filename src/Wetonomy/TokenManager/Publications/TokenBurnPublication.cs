using System.Numerics;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenBurnPublication
    {
        public BigInteger Amount { get; }

        public IAgentTokenKey From { get; }

        public TokenBurnPublication(BigInteger amount, IAgentTokenKey from)
        {
            Amount = amount;
            From = from;
        }
    }
}