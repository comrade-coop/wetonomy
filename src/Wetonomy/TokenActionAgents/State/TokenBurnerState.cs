using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.State
{
    public class TokenBurnerState : RecipientState
    {
        public HashSet<TokensTransferedNotification> TransferMessages = new HashSet<TokensTransferedNotification>();

        public bool GetTokens(IAgentTokenKey from, BigInteger amount, out IAgentTokenKey sender)
        {
            sender = default;
            TokensTransferedNotification element = TransferMessages.FirstOrDefault(x => x.From.Equals(from) && x.Amount == amount);
            if (element == null) return false;
            BigInteger current = element.Amount;
            if (current > amount)
            {
                //Not sure if we need this scenario
                var newTokensMsg = new TokensTransferedNotification(null, element.Amount, element.From, element.To);
                sender = element.To;
                TransferMessages.Add(newTokensMsg);
                TransferMessages.Remove(element);
                return true;
            }
            if (current == amount)
            {
                sender = element.To;
                TransferMessages.Remove(element);
                return true;
            }
            return false;
        }
    }
}
