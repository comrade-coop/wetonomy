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
		public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message)
		{
            var messagesResult = new List<object>();
            var publicationResult = new List<object>();
            if (state is TokenBurnerState burnerState)
            {
                BigInteger amount = message.Amount;
                while (amount > 0)
                {
                    IAgentTokenKey recipient = state.Recipients.First();
                    TokensMintedTriggerer element = burnerState.MintedMessages.FirstOrDefault(x => x.To.Equals(recipient));
                    BigInteger debt = element.Amount;
                    IAgentTokenKey sender = element.To;

                    BurnTokenMessage command;
                    TokensBurnedTriggerer command2;

                    if (debt <= amount)
                    {
                        state.Recipients.Remove(recipient);
                        burnerState.MintedMessages.Remove(element);
                        amount -= debt;
                        command = new BurnTokenMessage(debt, sender);
                        command2 = new TokensBurnedTriggerer(state.SelfId, debt, recipient);
                    }
                    else
                    {
                        element.Amount -= amount;
                        command = new BurnTokenMessage(amount, sender);
                        command2 = new TokensBurnedTriggerer(state.SelfId, amount, recipient);

                        amount = 0;
                    }
                    
                    messagesResult.Add(command);
                    publicationResult.Add(command2);
                }

            }
            return (messagesResult, publicationResult);
        }
	}
}
