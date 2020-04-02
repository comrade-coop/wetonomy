using System;
using System.Collections.Generic;
using System.Linq;

namespace Wetonomy.Voting
{
    
    public class MemberBasedVotingStategy : IVoteStategy<bool, bool>
    {
        public bool MakeDecision(IEnumerable<bool> votes)
        {
            int negative = votes.Aggregate(0, (sum, x) => x == false ? sum++ : sum);
            return negative > votes.Count() / 2;
        }
    }
}
