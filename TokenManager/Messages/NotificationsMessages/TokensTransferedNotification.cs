using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensTransferedNotification<T> : AbstractTriggerer
    {
        public BigInteger Amount { get; }
        public T From { get; }
        public T To { get; }

        public TokensTransferedNotification(BigInteger amount, T from, T to)
        {
            Amount = amount;
            From = from;
            To = to;
        }
    }
}
