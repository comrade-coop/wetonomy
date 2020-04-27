using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Publications
{
    class RecipientRemovedPublication
    {
        public IAgentTokenKey Recipient;

        public RecipientRemovedPublication(IAgentTokenKey recipient)
        {
            Recipient = recipient;
        }
    }
}
