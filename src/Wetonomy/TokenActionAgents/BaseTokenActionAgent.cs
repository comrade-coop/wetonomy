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
using Wetonomy.TokenActionAgents.Strategies;

namespace Wetonomy.TokenActionAgents
{
	public class BaseTokenActionAgent
    {
        public Task<AgentContext<RecipientState>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as RecipientState ?? new RecipientState();
            var context = new AgentContext<RecipientState>(agentState, self);

            if (message is AbstractTrigger msg)
            {
                var pair = new AgentTriggerPair(msg.Sender, message.GetType());
                if (context.State.TriggerToAction.ContainsKey(pair))
                {
                    (IList<object>, IList<object>) result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (var action in result.Item1)
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
                    context.State.Recipients = initMessage.Recipients;// ?? new List<IAgentTokenKey>();
                    context.State.SelfId = self.Issuer;

                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"AddRecipientMessage", context.IssueCapability(new[]{ typeof(AddRecipientMessage) }) },
                            {"RemoveRecipientMessage", context.IssueCapability(new[]{ typeof(RemoveRecipientMessage) }) },
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

                case AddRecipientMessage addMessage:
                    if (context.State.AddRecipient(addMessage.Recipient))
                    {
                        context.MakePublication(new RecipientAddedPublication(addMessage.Recipient));
                    }
                    break;

                case RemoveRecipientMessage removeMessage:
                    if (context.State.RemoveRecipient(removeMessage.Recipient))
                    {
                        context.MakePublication(new RecipientRemovedPublication(removeMessage.Recipient));
                    }
                    break;

                case AddTriggerToActionMessage addTriggerMessage:
                    context.State.TriggerToAction.Add(addTriggerMessage.Trigger, addTriggerMessage.Action);
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
