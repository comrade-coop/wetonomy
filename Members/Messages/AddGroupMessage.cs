using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Members.Messages
{
    public class AddGroupMessage
    {
        public string GroupAgentId { get; }

        public AddGroupMessage(string agentId)
        {
            GroupAgentId = agentId;
        }
    }
}
