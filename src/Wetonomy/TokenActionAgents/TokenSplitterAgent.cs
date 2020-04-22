using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Publications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents
{
    public class TokenSplitterAgent<T>: BaseTokenActionAgent<T> where T: IEquatable<T>
    {
        public new Task<AgentContext<RecipientState<T>>> Run(object state, AgentCapability self, object message)
        {

            var agentState = state as RecipientState<T> ?? new RecipientState<T>();
            var context = new AgentContext<RecipientState<T>>(agentState, self);

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
                default:
                    Task<AgentContext<RecipientState<T>>> secondaryContextTask = base.Run(agentState, self, message);
                    var secondaryContext = secondaryContextTask.GetAwaiter().GetResult();
                    context.MergeSecondaryContext(secondaryContext.GetCommands());
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
