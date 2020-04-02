using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Messages
{
    public class AddVoteMessage<V>
    {
        public V Vote { get; }

        public string DecisionId { get; }

        public AddVoteMessage(V vote, string decisionId)
        {
            Vote = vote;
            DecisionId = decisionId;
        }
    }
}
