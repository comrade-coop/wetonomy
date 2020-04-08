using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensBurnedNotification<T>: AbstractTriggerer
    {
        public T From { get; }

        public TokensBurnedNotification(string sender, BigInteger amount, T from)
        {
            Sender = sender;
            Amount = amount;
            From = from;
        }
    }
}
