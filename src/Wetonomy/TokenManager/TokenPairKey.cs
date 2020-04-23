using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Wetonomy.TokenManager
{
    public interface IAgentTokenKey
    {
        public string GetAgentId();

        //public T GetSender();
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

        public override int GetHashCode()
        {
            return AgentId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (SingleAngentTokenKey)obj;
            return AgentId == other.AgentId;
        }
    }

    public class TokenPairKey<T>: SingleAngentTokenKey
    {
        private readonly T Tag;

        public TokenPairKey(string agentId, T tag): base(agentId)
        {
            Tag = tag;
        }


        public T GetTag()
        {
            return Tag;
        }
    }
}
