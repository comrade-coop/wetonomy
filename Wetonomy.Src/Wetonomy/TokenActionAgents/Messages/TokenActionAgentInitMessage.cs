using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents.State;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class TokenActionAgentInitMessage<T> where T: IEquatable<T>
    {
        public AgentCapability TokenManagerAgentCapability { get; set; }
        public Dictionary<(string, Type), TriggeredAction<T>> TriggererToAction { get; set; }

        public TokenActionAgentInitMessage(
            AgentCapability tokenManagerAgentCapability,
            Dictionary<(string, Type), TriggeredAction<T>> triggererToAction)
        {
            TokenManagerAgentCapability = tokenManagerAgentCapability;
            TriggererToAction = triggererToAction;
        }
    }
}