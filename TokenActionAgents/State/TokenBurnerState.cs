using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.State
{
    public class TokenBurnerState<T> : RecipientState<T> where T : IEquatable<T>
    {
        public HashSet<TokensTransferedNotification<T>> TransferMessages = new HashSet<TokensTransferedNotification<T>>();

        public bool GetTokens(T from, BigInteger amount, out T sender)
        {
            sender = default;
            TokensTransferedNotification<T> element = TransferMessages.FirstOrDefault(x => x.From.Equals(from) && x.Amount == amount);
            if (element == null) return false;
            BigInteger current = element.Amount;
            if (current > amount)
            {
                //Not sure if we need this scenario
                var newTokensMsg = new TokensTransferedNotification<T>(element.Amount, element.From, element.To);
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
