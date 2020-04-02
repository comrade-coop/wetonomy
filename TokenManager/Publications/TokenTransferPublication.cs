using System.Numerics;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenTransferPublication<T>
    {
        public BigInteger Amount { get; }

        public T From { get; }

        public T To { get; }

        public TokenTransferPublication(BigInteger amount, T from, T to)
        {
            Amount = amount;
            From = from;
            To = to;
        }
    }
}