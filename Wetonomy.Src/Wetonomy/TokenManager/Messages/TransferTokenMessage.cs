using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    class TransferTokenMessage<T>
    {
        public BigInteger Amount { get; }
        public T From { get; }
        public T To { get; }

        public TransferTokenMessage(BigInteger amount, T from, T to)
        {
            Amount = amount;
            From = from;
            To = to;
        }
    }
}
