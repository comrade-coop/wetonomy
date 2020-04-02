using System.Numerics;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenBurnPublication<T>
    {
        public BigInteger Amount { get; }

        public T From { get; }

        public TokenBurnPublication(BigInteger amount, T from)
        {
            Amount = amount;
            From = from;
        }
    }
}