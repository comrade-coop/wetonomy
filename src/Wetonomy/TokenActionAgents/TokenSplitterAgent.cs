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
    public class TokenSplitterAgent: BaseTokenActionAgent
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
                    var result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (TransferTokenMessage action in result)
                    {
                        context.SendMessage(context.State.TokenManagerAgent, action, null);
                    }

                    return Task.FromResult(context);
                }
            }

            switch (message)
            {
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
