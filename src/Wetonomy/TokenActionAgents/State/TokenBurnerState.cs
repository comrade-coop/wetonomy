using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.State
{
    public class TokenBurnerState : RecipientState
    {
        public HashSet<TokensMintedTriggerer> MintedMessages = new HashSet<TokensMintedTriggerer>();

        public bool GetTokens(IAgentTokenKey from, BigInteger amount, out IAgentTokenKey sender)
        {
            sender = default;
            TokensMintedTriggerer element = MintedMessages.FirstOrDefault(x => x.To.Equals(from) && x.Amount == amount);
            if (element == null) return false;
            BigInteger current = element.Amount;
            if (current > amount)
            {
                //Not sure if we need this scenario
                var newTokensMsg = new TokensMintedTriggerer(element.Sender, element.Amount, element.To);
                sender = element.To;
                MintedMessages.Add(newTokensMsg);
                MintedMessages.Remove(element);
                return true;
            }
            if (current == amount)
            {
                sender = element.To;
                MintedMessages.Remove(element);
                return true;
            }
            return false;
        }
    }
}
