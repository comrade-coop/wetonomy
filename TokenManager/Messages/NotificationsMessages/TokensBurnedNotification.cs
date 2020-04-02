using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensBurnedNotification<T>: AbstractTriggerer
    {
        public BigInteger Amount { get; }
        public T From { get; }

        public TokensBurnedNotification(BigInteger amount, T from)
        {
            Amount = amount;
            From = from;
        }
    }
}
