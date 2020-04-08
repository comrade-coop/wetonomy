using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Messages.Notifications
{
    class TokensMintedTriggerer<T>: AbstractTriggerer
    {
        public T To { get; }

        public TokensMintedTriggerer(string sender, BigInteger amount, T to)
        {
            Sender = sender;
            Amount = amount;
            To = to;
        }
    }
}
