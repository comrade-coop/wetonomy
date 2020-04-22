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
    public class TokenMinterAgent<T>: BaseTokenActionAgent<T> where T: IEquatable<T>
    {
        public new Task<AgentContext<RecipientState<T>>> Run(object state, AgentCapability self, object message)
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
                        context.SendMessage(context.State.TokenManagerAgent, mintMsg, null);
                    }
                    //Publication
                    if (action is TokensMintedTriggerer<T> trigger)
                    {
                        context.MakePublication(trigger);
                    }
                }

                return Task.FromResult(context);
            }

            switch (message)
            {
                //case SomeMessage msg : break;
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
