using Apocryph.Agents.Testbed.Api;
using System;
using System.Threading.Tasks;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents
{
    public class TokenSplitterAgent<T> where T: IEquatable<T>
    {
        public Task<AgentContext<RecipientState<T>>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<RecipientState<T>>(state as RecipientState<T>, self);

            if (message is AbstractTrigger msg && context.State.TriggerToAction.ContainsKey((msg.Sender, message.GetType())))
            {
                var result = RecipientState<T>.TriggerCheck(context.State, msg.Sender, msg);

                foreach (TransferTokenMessage<T> action in result)
                {
                    context.SendMessage(context.State.TokenManagerAgent, action, null);
                }

                return Task.FromResult(context);
            }

            switch (message)
            {
                case TokenActionAgentInitMessage<T> organizationInitMessage:
                    context.State.TokenManagerAgent = organizationInitMessage.TokenManagerAgentCapability;
                    context.State.TriggerToAction = organizationInitMessage.TriggererToAction;
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
