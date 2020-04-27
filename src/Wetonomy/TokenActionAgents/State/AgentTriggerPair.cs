using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenActionAgents.State
{
    public class AgentTriggerPair
    {
        public string AgentId;

        public Type Trigger;
        public AgentTriggerPair(string agentId, Type trigger)
        {
            Trigger = trigger;

            AgentId = agentId;
        }

        public override int GetHashCode()
        {
            return AgentId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (AgentTriggerPair)obj;
            return AgentId == other.AgentId;
        }
    }
}
