using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.MembersAndGroups.Group.Messages
{
    public class AddMemberMessage
    {
        public string MemberAgentId { get; }

        // Stub implementation
        public AddMemberMessage(string agentId)
        {
            MemberAgentId = agentId;
        }
    }
}
