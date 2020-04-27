using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages.NotificationsMessages
{
    public class TokensBurnedNotification: AbstractTrigger
    {
        public IAgentTokenKey From { get; }

        public TokensBurnedNotification(string sender, BigInteger amount, IAgentTokenKey from): base(amount, sender)
        {
            From = from;
        }
    }
}
