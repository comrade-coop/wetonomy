using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Apocryph.FunctionApp.Agent;
using Wetonomy.Groups.Messages;

namespace Wetonomy.Groups
{
    public static class GroupAgent
    {
        public class GroupState
        {
            public HashSet<string> Members = new HashSet<string>();
            public HashSet<AgentCapability> Capabilities = new HashSet<AgentCapability>();
        }

        public static AgentContext<GroupState> Run(object state, string sender, object message)
        {
            var context = new AgentContext<GroupState>(state as GroupState);

            switch (message)
            {
                case AddMemberMessage addMemberMessage:
                    context.State.Members.Add(addMemberMessage.MemberAgentId);

                    break;

                case RemoveMemberMessage removeMemberMessage:
                    context.State.Members.Remove(removeMemberMessage.MemberAgentId);

                    break;
            }

            return context;
        }
    }
}
