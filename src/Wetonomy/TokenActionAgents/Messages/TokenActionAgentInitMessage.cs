using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Strategies;
using Wetonomy.TokenManager;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class TokenActionAgentInitMessage
    {
        public AgentCapability TokenManagerAgentCapability { get; }

        public AgentCapability CreatorAgentCapability { get; }

        public Dictionary<AgentTriggerPair, ITriggeredAction> TriggererToAction { get; }

        public List<IAgentTokenKey> Recipients { get; }

        public HashSet<string> Subscription { get; }

        public TokenActionAgentInitMessage(
            AgentCapability tokenManagerAgentCapability,
            AgentCapability creatorAgentCapability,
            Dictionary<AgentTriggerPair, ITriggeredAction> triggererToAction,
            List<IAgentTokenKey> recipients = null,
            HashSet<string> subscription = null)
        {
            CreatorAgentCapability = creatorAgentCapability;
            TokenManagerAgentCapability = tokenManagerAgentCapability;
            TriggererToAction = triggererToAction;
            Recipients = recipients;
            Subscription = subscription;
        }
    }
}