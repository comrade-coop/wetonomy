using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensMintedNotification<T> : AbstractTriggerer
    {
        public BigInteger Amount { get; }

        public T To { get; }

        public TokensMintedNotification(BigInteger amount, T to)
        {
            Amount = amount;
            To = to;
        }
    }
}
