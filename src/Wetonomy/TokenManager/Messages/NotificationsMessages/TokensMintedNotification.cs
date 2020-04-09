using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensMintedNotification<T> : AbstractTrigger
    {

        public T To { get; }

        public TokensMintedNotification(string sender, BigInteger amount, T to)
        {
            Sender = sender;
            Amount = amount;
            To = to;
        }
    }
}
