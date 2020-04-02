using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Messages.Notifications
{
    public class TokensBurnedTriggerer<T>: AbstractTriggerer
    {
        public BigInteger Amount { get; }
        public T From { get; }

        public TokensBurnedTriggerer(BigInteger amount, T from)
        {
            Amount = amount;
            From = from;
        }
    }
}
