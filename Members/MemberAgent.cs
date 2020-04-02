using System;
using System.Collections.Generic;
using Apocryph.FunctionApp.Agent;
using Wetonomy.Members.Messages;

namespace Wetonomy.Members
{
    public static class MemberAgent
    {
        public class MemberState
        {
            public HashSet<string> Groups = new HashSet<string>();
            public Dictionary<object, AgentCapability> Capabilities = new Dictionary<object, AgentCapability>();
        }

        public static AgentContext<MemberState> Run(IAgentContext<MemberState> state, string sender, object message)
        {
            var context = new AgentContext<MemberState>(state as MemberState);
            switch (message)
            {
                case AddGroupMessage addGroupMessage:
                    context.State.Groups.Add(addGroupMessage.GroupAgentId);
                    
                    break;

                case RemoveMemberMessage removeGroupMessage:
                    context.State.Groups.Remove(removeGroupMessage.GroupAgentId);
                    
                    break;
            }

            return context;
        }
    }
}
