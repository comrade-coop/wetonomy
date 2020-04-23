using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Messages.Notifications
{
    public class TokensBurnedTriggerer: AbstractTrigger
    {
        public IAgentTokenKey From { get; }

        public TokensBurnedTriggerer(string sender, BigInteger amount, IAgentTokenKey from)
        {
            Sender = sender;
            Amount = amount;
            From = from;
        }
    }
}
