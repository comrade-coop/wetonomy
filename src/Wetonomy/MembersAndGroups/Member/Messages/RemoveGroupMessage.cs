using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.MembersAndGroups.Member.Messages
{
    class RemoveMemberMessage
    {
        public string GroupAgentId { get; }

        public RemoveMemberMessage(string agentId)
        {
            GroupAgentId = agentId;
        }
    }
}
