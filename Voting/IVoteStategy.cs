using System.Collections.Generic;

namespace Wetonomy.Voting
{
    public interface IVoteStategy<T,V>
    {
        public T MakeDecision(IEnumerable<V> votes);
    }
}