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
    public class TokenMinterAgent: BaseTokenActionAgent
    {
        public new Task<AgentContext<RecipientState>> Run(object state, AgentCapability self, object message)
        {

            var agentState = state as RecipientState ?? new RecipientState();
            var context = new AgentContext<RecipientState>(agentState, self);

            if (message is AbstractTrigger msg)
            {
                var pair = new AgentTriggerPair(msg.Sender, message.GetType());
                if (context.State.TriggerToAction.ContainsKey(pair))
                {
                    (IList<object>, IList<object>) result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (MintTokenMessage action in result.Item1)
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
                //case SomeMessage msg : break;
                default:
                    Task<AgentContext<RecipientState>> secondaryContextTask = base.Run(agentState, self, message);
                    var secondaryContext = secondaryContextTask.GetAwaiter().GetResult();
                    context.MergeSecondaryContext(secondaryContext.GetCommands());
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
