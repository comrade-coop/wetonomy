using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensTransferedNotification: AbstractTrigger
    {
        public IAgentTokenKey From { get; }
        public IAgentTokenKey To { get; }

        public TokensTransferedNotification(string sender, BigInteger amount, IAgentTokenKey from, IAgentTokenKey to) : base(amount, sender)
        {
            From = from;
            To = to;
        }
    }
}
