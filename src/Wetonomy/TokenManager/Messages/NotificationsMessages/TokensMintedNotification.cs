using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensMintedNotification : AbstractTrigger
    {

        public IAgentTokenKey To { get; }

        public TokensMintedNotification(string sender, BigInteger amount, IAgentTokenKey to)
        {
            Sender = sender;
            Amount = amount;
            To = to;
        }
    }
}
