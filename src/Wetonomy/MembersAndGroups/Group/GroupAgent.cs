using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Apocryph.Agents.Testbed.Api;
using Wetonomy.MembersAndGroups.Group.Messages;

namespace Wetonomy.MembersAndGroups.Group
{
    public class GroupAgent
    {
        public class GroupState
        {
            public HashSet<string> Members = new HashSet<string>();
            public HashSet<AgentCapability> Capabilities = new HashSet<AgentCapability>();
        }

        public Task<AgentContext<GroupState>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as GroupState ?? new GroupState();
            var context = new AgentContext<GroupState>(agentState, self);

            switch (message)
            {
                case InitWetonomyAgentMessage initMsg:
                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"AddMemberMessage", context.IssueCapability(new[]{ typeof(AddMemberMessage) }) },
                            {"RemoveMemberMessage", context.IssueCapability(new[]{ typeof(RemoveMemberMessage) }) },
                        }
                    };

                    context.SendMessage(initMsg.CreatorAgentCapability, distributeCapabilityMessage, null);
                    break;

                case AddMemberMessage addMemberMessage:
                    context.State.Members.Add(addMemberMessage.MemberAgentId);
                    break;

                case RemoveMemberMessage removeMemberMessage:
                    context.State.Members.Remove(removeMemberMessage.MemberAgentId);
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
