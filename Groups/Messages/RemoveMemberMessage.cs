using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Groups.Messages
{
    class RemoveMemberMessage
    {
        public string MemberAgentId { get; }

        // Stub implementation
        public RemoveMemberMessage(string agentId)
        {
            MemberAgentId = agentId;
        }
    }
}
