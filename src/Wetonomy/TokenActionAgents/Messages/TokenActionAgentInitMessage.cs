using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Strategies;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class TokenActionAgentInitMessage<T> where T: IEquatable<T>
    {
        public AgentCapability TokenManagerAgentCapability { get; }

        public AgentCapability CreatorAgentCapability { get; }

        public Dictionary<(string, Type), ITriggeredAction<T>> TriggererToAction { get; }

        public HashSet<string> Subscription { get; }

        public TokenActionAgentInitMessage(
            AgentCapability tokenManagerAgentCapability,
            AgentCapability creatorAgentCapability,
            Dictionary<(string, Type), ITriggeredAction<T>> triggererToAction,
            HashSet<string> subscription)
        {
            CreatorAgentCapability = creatorAgentCapability;
            TokenManagerAgentCapability = tokenManagerAgentCapability;
            TriggererToAction = triggererToAction;
            Subscription = subscription;
        }
    }
}