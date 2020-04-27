using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Messages.Notifications
{
    public class TokensMintedTriggerer: AbstractTrigger
    {
        public IAgentTokenKey To { get; }

        public TokensMintedTriggerer(string sender, BigInteger amount, IAgentTokenKey to) : base(amount, sender)
        {
            To = to;
        }
    }
}
