using Apocryph.Agents.Testbed.Api;

namespace Wetonomy.TokenManager.Messages
{
    public class TokenManagerInitMessage
    {
        public string Id;
        public AgentCapability OrganizationAgentCapability { get; set; }
    }
}