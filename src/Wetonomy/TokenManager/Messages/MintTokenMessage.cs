using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public class MintTokenMessage<T>
    {
        public BigInteger Amount { get; }
        public T To { get; }

        public MintTokenMessage(BigInteger amount, T to)
        {
            Amount = amount;
            To = to;
        }
    }
}
