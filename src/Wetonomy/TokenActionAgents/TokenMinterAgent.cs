using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents
{
    public class TokenMinterAgent<T> where T: IEquatable<T>
    {
        public Task<AgentContext<RecipientState<T>>> Run(object state, AgentCapability self, object message)
        {

            var agentState = state as RecipientState<T> ?? new RecipientState<T>();
            var context = new AgentContext<RecipientState<T>>(agentState, self);

            if (message is AbstractTrigger msg && context.State.TriggerToAction.ContainsKey((msg.Sender, message.GetType())))
            {
                var result = RecipientState<T>.TriggerCheck(context.State, msg.Sender, msg);

                foreach (var action in result)
                {
                    if(action is MintTokenMessage<T> mintMsg)
                    {
                        context.SendMessage(context.State.TokenManagerAgent, action, null);
                    }
                    //Publication
                    //if(action is TokensMintedTriggerer<T> trigger)
                    //{
                    //    context.SendMessage(context.State.TokenManagerAgent, action, null);
                    //}
                }

                return Task.FromResult(context);
            }

            switch (message)
            {

                case TokenActionAgentInitMessage<T> initMessage:
                    context.State.TokenManagerAgent = initMessage.TokenManagerAgentCapability;
                    context.State.TriggerToAction = initMessage.TriggererToAction;

                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"AddRecipientMessage", context.IssueCapability(new[]{ typeof(AddRecipientMessage<T>) }) },
                            {"RemoveRecipientMessage", context.IssueCapability(new[]{ typeof(RemoveRecipientMessage<T>) }) },
                            {"AddTriggerToActionMessage", context.IssueCapability(new[]{ typeof(AddTriggerToActionMessage<T>) }) },
                        }
                    };

                    context.SendMessage(initMessage.CreatorAgentCapability, distributeCapabilityMessage, null);
                    break;

                case AddRecipientMessage<T> addMessage:
                    if (context.State.AddRecipient(addMessage.Recipient))
                    {
                        context.MakePublication(new RecipientAddedPublication<T>(addMessage.Recipient));
                    }
                    break;

                case RemoveRecipientMessage<T> removeMessage:
                    if (context.State.RemoveRecipient(removeMessage.Recipient))
                    {
                        context.MakePublication(new RecipientRemovedPublication<T>(removeMessage.Recipient));
                    }
                    break;

                case AddTriggerToActionMessage<T> addTriggerMessage:
                    context.State.TriggerToAction.Add(addTriggerMessage.Trigger, addTriggerMessage.Action);
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
