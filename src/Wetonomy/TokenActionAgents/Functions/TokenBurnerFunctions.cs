using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using System.Numerics;
using System.Linq;
using Wetonomy.TokenActionAgents.Messages.Notifications;

namespace Wetonomy.TokenActionAgents.Functions
{
    public static class TokenBurnerFunctions<T> where T : IEquatable<T>
    {
        public static IList<object> SelfBurn(RecipientState<T> _, AbstractTrigger message)
        {
            if(message is TokensTransferedNotification<T> msg)
            {
                var command = new BurnTokenMessage<T>(msg.Amount, msg.To);
                return new List<object>() { command };
            }
            return null;
        }

        public static IList<object> SequentialBurn(RecipientState<T> state, AbstractTrigger message)
        {
            var result = new List<object>();
            if(state is TokenBurnerState<T> burnerState)
            {
                BigInteger amount = message.Amount;
                while(amount > 0)
                {
                    T recipient = state.Recipients.First();
                    TokensTransferedNotification<T> element = burnerState.TransferMessages.FirstOrDefault(x => x.From.Equals(recipient));
                    BigInteger debt = element.Amount;
                    T sender = element.To;
                    if( debt <= amount)
                    {
                        state.Recipients.Remove(recipient);
                        burnerState.TransferMessages.Remove(element);
                        amount -= debt;
                    }
                    else
                    {

                    }
                    var command = new BurnTokenMessage<T>(debt, sender);
                    var command2 = new TokensBurnedTriggerer<T>(null, debt, recipient);

                    result.Add(command);
                    result.Add(command2);
                }

            }
            return result;
        }
    }
}
