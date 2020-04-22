using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenManager
{
    public interface IAgentTokenKey
    {
        public string GetAgentId();
    }

    public class SingleAngentTokenKey : IAgentTokenKey
    {
        protected readonly string AgentId;

        public SingleAngentTokenKey(string agentId)
            => AgentId = agentId;


        public string GetAgentId()
        {
            return AgentId;
        }
    }

    public class TokenPair<T>: SingleAngentTokenKey
    {
        private readonly T Tag;

        public TokenPair(string agentId, T tag): base(agentId)
        {
            Tag = tag;
        }


        public T GetTag()
        {
            return Tag;
        }
    }
}
