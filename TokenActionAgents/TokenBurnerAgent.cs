using Apocryph.FunctionApp.Agent;
using System;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using System.Linq;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Functions;

namespace Wetonomy.TokenActionAgents
{
    class TokenBurnerAgent<T> where T : IEquatable<T>
    {
        public static AgentContext<TokenBurnerState<T>> Run(object state, AgentCapability sender, object message)
        {
            var context = new AgentContext<TokenBurnerState<T>>(state as TokenBurnerState<T>);

            if (message is AbstractTriggerer msg && context.State.TriggererToAction.ContainsKey((sender.AgentId, message.GetType())))
            {
                var result = RecipientState<T>.TriggerCheck(context.State, sender.AgentId, msg);

                foreach (BurnTokenMessage<T> action in result)
                {
                    context.SendMessage(context.State.TokenManagerAgent, action, null);
                }

                return context;
            }

            switch(message)
            {
                case TokenActionAgentInitMessage<T> organizationInitMessage:
                    context.State.TokenManagerAgent = organizationInitMessage.TokenManagerAgentCapability;
                    context.State.TriggererToAction = organizationInitMessage.TriggererToAction;
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
            }

            return context;
        }

    }
}
