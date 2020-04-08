using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apocryph.Agents.Testbed.Api;
using Wetonomy.Members.Messages;

namespace Wetonomy.Members
{
    public static class MemberAgent
    {
        public class MemberState
        {
            public HashSet<string> Groups = new HashSet<string>();
            public Dictionary<object, AgentCapability> Capabilities = new Dictionary<object, AgentCapability>();

            public bool Equals([AllowNull] MemberState other)
            {
                throw new NotImplementedException();
            }
        }

        public static AgentContext<MemberState> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<MemberState>(state as MemberState, self);
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
