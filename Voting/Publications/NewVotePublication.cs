using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Publications
{
    public class NewVotePublication<V>
    {
        public string DecisionId { get; }
        public V Vote { get; }

        public NewVotePublication(string id, V vote)
        {
            DecisionId = id;
            Vote = vote;
        }
    }
}
