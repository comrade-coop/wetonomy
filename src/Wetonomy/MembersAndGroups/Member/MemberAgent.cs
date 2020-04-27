using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apocryph.Agents.Testbed.Api;
using Wetonomy.MembersAndGroups.Member.Messages;

namespace Wetonomy.MembersAndGroups.Member
{
    public class MemberAgent
    {
        public class MemberState
        {
            public HashSet<string> Groups = new HashSet<string>();
            public Dictionary<object, AgentCapability> Capabilities = new Dictionary<object, AgentCapability>();
        }

        public Task<AgentContext<MemberState>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as MemberState ?? new MemberState();
            var context = new AgentContext<MemberState>(agentState, self);

            switch (message)
            {
                case InitWetonomyAgentMessage initMsg:
                    //context.SubscribeUserInput()
                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"AddGroupMessage", context.IssueCapability(new[]{ typeof(AddGroupMessage) }) },
                            {"RemoveMemberMessage", context.IssueCapability(new[]{ typeof(RemoveMemberMessage) }) },
                        }
                    };

                    context.SendMessage(initMsg.CreatorAgentCapability, distributeCapabilityMessage, null);
                    break;

                case AddGroupMessage addGroupMessage:
                    context.State.Groups.Add(addGroupMessage.GroupAgentId);
                    break;

                case RemoveMemberMessage removeGroupMessage:
                    context.State.Groups.Remove(removeGroupMessage.GroupAgentId);
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
