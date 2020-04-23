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
                    var result = RecipientState.TriggerCheck(context.State, pair, msg);

                    foreach (var action in result)
                    {
                        if (action is MintTokenMessage mintMsg)
                        {
                            context.SendMessage(context.State.TokenManagerAgent, mintMsg, null);
                        }
                        //Publication
                        if (action is TokensMintedTriggerer trigger)
                        {
                            context.MakePublication(trigger);
                        }
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
