using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Wetonomy.TokenActionAgents.Strategies;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.State
{   
    public class RecipientState
    {

        public AgentCapability TokenManagerAgent;

        //using string for Agent identifier because we do not need capabilities,
        //the trigger is notification that something has happened
        public Dictionary<AgentTriggerPair, ITriggeredAction> TriggerToAction;

        public List<IAgentTokenKey> Recipients = new List<IAgentTokenKey>();

        public string SelfId;

        public bool AddRecipient(IAgentTokenKey recipient)
        {
            Recipients.Add(recipient);

            return true;
        }

        public bool RemoveRecipient(IAgentTokenKey recipient)
        {
            int index = Recipients.FindIndex(x => x.Equals(recipient));
            if (index == -1) return false;
            Recipients.RemoveAt(index);

            return true;
        }


        public static (IList<object>, IList<object>) TriggerCheck(RecipientState state, AgentTriggerPair pair, AbstractTrigger message)
        {
            ITriggeredAction func = state.TriggerToAction[pair];

            (IList<object>, IList<object>) result = func.Execute(state, message);

            return (result.Item1 ?? new List<object>(), result.Item2 ?? new List<object>());

        }
    }
}
