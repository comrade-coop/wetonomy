using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents.Strategies;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.State
{   
    public class RecipientState<T> where T: IEquatable<T>
    {

        public AgentCapability TokenManagerAgent;

        //using string for Agent identifier because we do not need capabilities,
        //the trigger is notification that something has happened
        public Dictionary<(string, Type), ITriggeredAction<T>> TriggerToAction;

        public List<T> Recipients = new List<T>();

        public bool AddRecipient(T recipient)
        {
            Recipients.Add(recipient);

            return true;
        }

        public bool RemoveRecipient(T recipient)
        {
            int index = Recipients.FindIndex(x => x.Equals(recipient));
            if (index == -1) return false;
            Recipients.RemoveAt(index);

            return true;
        }


        public static IList<object> TriggerCheck(RecipientState<T> state, string sender, AbstractTrigger message)
        {
            ITriggeredAction<T> func = state.TriggerToAction[(sender, message.GetType())];

            IList<object> result = func.Execute(state, message);

            return result;

        }
    }
}
