using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensTransferedNotification<T> : AbstractTrigger
    {
        public T From { get; }
        public T To { get; }

        public TokensTransferedNotification(string sender, BigInteger amount, T from, T to)
        {
            Sender = sender;
            Amount = amount;
            From = from;
            To = to;
        }
    }
}
