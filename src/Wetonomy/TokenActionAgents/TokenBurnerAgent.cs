using Apocryph.Agents.Testbed.Api;
using System;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using System.Linq;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Functions;
using System.Threading.Tasks;

namespace Wetonomy.TokenActionAgents
{
    public class TokenBurnerAgent<T> where T : IEquatable<T>
    {
        public Task<AgentContext<TokenBurnerState<T>>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<TokenBurnerState<T>>(state as TokenBurnerState<T>, self);

            if (message is AbstractTrigger msg && context.State.TriggerToAction.ContainsKey((msg.Sender, message.GetType())))
            {
                var result = RecipientState<T>.TriggerCheck(context.State, msg.Sender, msg);

                foreach (BurnTokenMessage<T> action in result)
                {
                    context.SendMessage(context.State.TokenManagerAgent, action, null);
                    //here we need to make a publication TokensBurnedTriggerer
                }

                return Task.FromResult(context);
            }

            switch(message)
            {
                case TokenActionAgentInitMessage<T> organizationInitMessage:
                    context.State.TokenManagerAgent = organizationInitMessage.TokenManagerAgentCapability;
                    context.State.TriggerToAction = organizationInitMessage.TriggererToAction;
                    break;
                case TokensTransferedNotification<T> transferedMessage:
                    if (context.State.AddRecipient(transferedMessage.From))
                    {
                        context.State.TransferMessages.Add(transferedMessage);
                        context.MakePublication(new RecipientAddedPublication<T>(transferedMessage.From));
                    }
                    break;

                case GetTokensMessage<T> getTokensMessage:
                    T agentSender;
                    if (context.State.GetTokens(getTokensMessage.Recipient, getTokensMessage.Amount, out agentSender))
                    {
                        var transfer = new TransferTokenMessage<T>(getTokensMessage.Amount, agentSender, getTokensMessage.Recipient);
                        context.SendMessage(null, transfer, null);
                    }
                    break;

                case AddTriggerToActionMessage<T> addTriggerMessage:
                    context.State.TriggerToAction.Add(addTriggerMessage.Trigger,addTriggerMessage.Action);
                    break;
            }

            return Task.FromResult(context);
        }

    }
}
