using Apocryph.Agents.Testbed.Api;
using System;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using System.Linq;
using Wetonomy.TokenActionAgents.State;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wetonomy.TokenActionAgents
{
    public class TokenBurnerAgent<T> where T : IEquatable<T>
    {
        public Task<AgentContext<TokenBurnerState<T>>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as TokenBurnerState<T> ?? new TokenBurnerState<T>();
            var context = new AgentContext<TokenBurnerState<T>>(agentState, self);

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
                case TokenActionAgentInitMessage<T> initMessage:
                    context.State.TokenManagerAgent = initMessage.TokenManagerAgentCapability;
                    context.State.TriggerToAction = initMessage.TriggererToAction;
                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"GetTokensMessage", context.IssueCapability(new[]{ typeof(GetTokensMessage<T>) }) },
                            {"AddTriggerToActionMessage", context.IssueCapability(new[]{ typeof(AddTriggerToActionMessage<T>) }) },
                        }
                    };
                    if (initMessage.Subscription != null)
                    {
                        foreach (var agent in initMessage.Subscription)
                        {
                            context.AddSubscription(agent);
                        }
                    }
                    context.SendMessage(initMessage.CreatorAgentCapability, distributeCapabilityMessage, null);
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
