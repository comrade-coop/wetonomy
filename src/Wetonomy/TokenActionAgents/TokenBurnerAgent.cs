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
                    (IList<object>, IList<object>) result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (BurnTokenMessage action in result.Item1)
                    {
                        context.SendMessage(context.State.TokenManagerAgent, action, null);
                    }

                    foreach (var publication in result.Item2)
                    {
                        context.MakePublication(publication);
                    }

                    return Task.FromResult(context);
                }
            }

            switch (message)
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

                //Tokens are minted on burner's addres so that he can burn them, 
                //but they are actually for the user TokensMintedTriggerer.To
                //He is added as recipient and he can take ownership of the tokens with GetTokensMessage
                //This is done so that the system can work automaticly
                case TokensMintedTriggerer transferedMessage:
                    if (context.State.AddRecipient(transferedMessage.To))
                    {
                        context.State.MintedMessages.Add(transferedMessage);
                        context.MakePublication(new RecipientAddedPublication(transferedMessage.To));
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
