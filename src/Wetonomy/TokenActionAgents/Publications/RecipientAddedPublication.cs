using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Publications
{
    class RecipientAddedPublication
    {
        public IAgentTokenKey Recipient;

        public RecipientAddedPublication(IAgentTokenKey recipient)
        {
            Recipient = recipient;
        }
    }
}
