using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class AddRecipientMessage
    {
        public IAgentTokenKey Recipient;

        public AddRecipientMessage(IAgentTokenKey recipient)
        {
            Recipient = recipient;
        }
    }
}
