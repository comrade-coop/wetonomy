using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Burn
{
	public class SequentialBurnStrategy<T> : ITriggeredAction<T> where T : IEquatable<T>
	{
		public IList<object> Execute(RecipientState<T> state, AbstractTrigger message)
		{
            var result = new List<object>();
            if (state is TokenBurnerState<T> burnerState)
            {
                BigInteger amount = message.Amount;
                while (amount > 0)
                {
                    T recipient = state.Recipients.First();
                    TokensTransferedNotification<T> element = burnerState.TransferMessages.FirstOrDefault(x => x.From.Equals(recipient));
                    BigInteger debt = element.Amount;
                    T sender = element.To;

                    BurnTokenMessage<T> command;
                    TokensBurnedTriggerer<T> command2;

                    if (debt <= amount)
                    {
                        state.Recipients.Remove(recipient);
                        burnerState.TransferMessages.Remove(element);
                        amount -= debt;
                        command = new BurnTokenMessage<T>(debt, sender);
                        command2 = new TokensBurnedTriggerer<T>(null, debt, recipient);
                    }
                    else
                    {
                        element.Amount -= amount;
                        amount = 0;
                        command = new BurnTokenMessage<T>(amount, sender);
                        command2 = new TokensBurnedTriggerer<T>(null, amount, recipient);
                    }
                    
                    result.Add(command);
                    result.Add(command2);
                }

            }
            return result;
        }
	}
}
