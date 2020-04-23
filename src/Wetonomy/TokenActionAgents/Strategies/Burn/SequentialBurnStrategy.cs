using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Burn
{
	public class SequentialBurnStrategy : ITriggeredAction
	{
		public IList<object> Execute(RecipientState state, AbstractTrigger message)
		{
            var result = new List<object>();
            if (state is TokenBurnerState burnerState)
            {
                BigInteger amount = message.Amount;
                while (amount > 0)
                {
                    IAgentTokenKey recipient = state.Recipients.First();
                    TokensTransferedNotification element = burnerState.TransferMessages.FirstOrDefault(x => x.From.Equals(recipient));
                    BigInteger debt = element.Amount;
                    IAgentTokenKey sender = element.To;

                    BurnTokenMessage command;
                    TokensBurnedTriggerer command2;

                    if (debt <= amount)
                    {
                        state.Recipients.Remove(recipient);
                        burnerState.TransferMessages.Remove(element);
                        amount -= debt;
                        command = new BurnTokenMessage(debt, sender);
                        command2 = new TokensBurnedTriggerer(state.SelfId, debt, recipient);
                    }
                    else
                    {
                        element.Amount -= amount;
                        amount = 0;
                        command = new BurnTokenMessage(amount, sender);
                        command2 = new TokensBurnedTriggerer(state.SelfId, amount, recipient);
                    }
                    
                    result.Add(command);
                    result.Add(command2);
                }

            }
            return result;
        }
	}
}
