using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    public class BurnTokenMessage<T>
    {
        public BigInteger Amount { get; }
        public T From { get; }

        public BurnTokenMessage(BigInteger amount, T from)
        {
            Amount = amount;
            From = from;
        }
    }
}
