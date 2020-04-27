using Apocryph.Agents.Testbed.Api;
using Apocryph.Agents.Testbed.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wetonomy.MembersAndGroups.Messages;

namespace Wetonomy.MembersAndGroups
{
	public class MemberAndGroupFactoryAgent: ForwardingAgent
	{
		public class FactoryState: BaseState
		{
			public Dictionary<string, Dictionary<string, AgentCapability>> AgentToCapabilities;

			public FactoryState()
			{
				AgentToCapabilities = new Dictionary<string, Dictionary<string, AgentCapability>>();
			}
		}
		public Task<AgentContext<FactoryState>> Run(object state, AgentCapability self, object message)
		{
			var agentState = state as FactoryState ?? new FactoryState();
			var context = new AgentContext<FactoryState>(agentState, self);

			switch (message)
			{
				case InitWetonomyAgentMessage initMsg:
					var distributeCapabilityMessage = new DistributeCapabilitiesMessage
					{
						Id = self.Issuer,
						AgentCapabilities = new Dictionary<string, AgentCapability>() {
							{"CreateGroupMessage", context.IssueCapability(new[]{ typeof(CreateGroupMessage) }) },
							{"CreateMemberMessage", context.IssueCapability(new[]{ typeof(CreateMemberMessage) })},
							{"RemoveRefMessage", context.IssueCapability(new[]{ typeof(RemoveRefMessage) }) },
							{"GetCapabilityMessage", context.IssueCapability(new[]{ typeof(GetCapabilityMessage) })},
							{"ForwardMessage", context.IssueCapability(new[]{ typeof(ForwardMessage) })}
						}
					};
					context.SendMessage(initMsg.CreatorAgentCapability, distributeCapabilityMessage, null);
					break;

				case CreateGroupMessage createGroupMsg:
					// should use created one
					var distribute = context.IssueCapability(new[] { typeof(DistributeCapabilitiesMessage)});
					context.CreateAgent(createGroupMsg.Id, "GroupAgent", distribute, null);
					break;

				case CreateMemberMessage createMemberMessage:
					// should use created one
					var distribute2 = context.IssueCapability(new[] { typeof(DistributeCapabilitiesMessage) });
					context.CreateAgent(createMemberMessage.Id, "MemberAgent", distribute2, null);
					break;

				case RemoveRefMessage removeMemberMessage:
					context.State.AgentToCapabilities.Remove(removeMemberMessage.Id);
					break;

				// for forwarding
				case GetCapabilityMessage getCapabilitiesMessage:
					var capability = context.IssueCapability(new[] { getCapabilitiesMessage.CapabilityType });
					var distributeCapabilityMsg = new DistributeCapabilitiesMessage
					{
						Id = getCapabilitiesMessage.Sender,
						AgentCapabilities = new Dictionary<string, AgentCapability>() {
							{getCapabilitiesMessage.CapabilityType.Name, capability },
						}
					};
					//check if Type.Name works as expected
					context.SendMessage(null, distributeCapabilityMsg, null);
					break;

				case DistributeCapabilitiesMessage distributeCapabilitiesMsg:
					context.State.AgentToCapabilities.Add(distributeCapabilitiesMsg.Id, distributeCapabilitiesMsg.AgentCapabilities);
					break;

				default:
					Task<AgentContext<BaseState>> secondaryContextTask =  base.Run(agentState, self, message);
					var secondaryContext= secondaryContextTask.GetAwaiter().GetResult();
					context.MergeSecondaryContext(secondaryContext.GetCommands());
					break;
			}

			return Task.FromResult(context);
		}
	}
}
