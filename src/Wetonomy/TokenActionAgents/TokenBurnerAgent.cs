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
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.Strategies;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents
{
    public class TokenBurnerAgent
    {
        public Task<AgentContext<TokenBurnerState>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as TokenBurnerState ?? new TokenBurnerState();
            var context = new AgentContext<TokenBurnerState>(agentState, self);

            if (message is AbstractTrigger msg)
            {
                var pair = new AgentTriggerPair(msg.Sender, message.GetType());
                if (context.State.TriggerToAction.ContainsKey(pair))
                {
                    var result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (var action in result)
                    {
                        if (action is BurnTokenMessage burnMsg)
                        {
                            context.SendMessage(context.State.TokenManagerAgent, burnMsg, null);
                        }
                        //Publication
                        if (action is TokensBurnedTriggerer trigger)
                        {
                            context.MakePublication(trigger);
                        }
                    }

                    return Task.FromResult(context);
                }
            }
            switch(message)
            {
                case TokenActionAgentInitMessage initMessage:
                    context.State.TokenManagerAgent = initMessage.TokenManagerAgentCapability;
                    context.State.TriggerToAction = initMessage.TriggererToAction ?? new Dictionary<AgentTriggerPair, ITriggeredAction>();
                    context.State.SelfId = self.Issuer;

                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"GetTokensMessage", context.IssueCapability(new[]{ typeof(GetTokensMessage) }) },
                            {"AddTriggerToActionMessage", context.IssueCapability(new[]{ typeof(AddTriggerToActionMessage) }) },
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

                case TokensTransferedNotification transferedMessage:
                    if (context.State.AddRecipient(transferedMessage.From))
                    {
                        context.State.TransferMessages.Add(transferedMessage);
                        context.MakePublication(new RecipientAddedPublication(transferedMessage.From));
                    }
                    break;

                case GetTokensMessage getTokensMessage:
                    IAgentTokenKey agentSender;
                    if (context.State.GetTokens(getTokensMessage.Recipient, getTokensMessage.Amount, out agentSender))
                    {
                        var transfer = new TransferTokenMessage(getTokensMessage.Amount, agentSender, getTokensMessage.Recipient);
                        context.SendMessage(null, transfer, null);
                    }
                    break;

                case AddTriggerToActionMessage addTriggerMessage:
                    context.State.TriggerToAction.Add(addTriggerMessage.Trigger,addTriggerMessage.Action);
                    break;
            }

            return Task.FromResult(context);
        }

    }
}
