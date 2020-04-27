using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Messages
{
    class RemoveRecipientMessage
    {
        public IAgentTokenKey Recipient;

        public RemoveRecipientMessage(IAgentTokenKey recipient)
        {
            Recipient = recipient;
        }
    }
}
