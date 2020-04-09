using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Messages.Notifications
{
    public class TokensBurnedTriggerer<T>: AbstractTrigger
    {
        public T From { get; }

        public TokensBurnedTriggerer(string sender, BigInteger amount, T from)
        {
            Sender = sender;
            Amount = amount;
            From = from;
        }
    }
}
