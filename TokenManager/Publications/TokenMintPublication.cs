using System.Collections.Generic;
using System.Numerics;
using Apocryph.FunctionApp.Model;

namespace Wetonomy.TokenManager.Publications
{
    public class TokenMintPublication<T>
    {
        public BigInteger Amount { get; }

        public T To { get; }

        public TokenMintPublication(BigInteger amount, T to)
        {
            Amount = amount;
            To = to;
        }
    }
}