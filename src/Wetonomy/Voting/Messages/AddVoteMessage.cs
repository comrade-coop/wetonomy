using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Messages
{
    public class AddVoteMessage<V>
    {
        public string Sender{ get; }
        public V Vote { get; }

        public string DecisionId { get; }

        public AddVoteMessage(string sender, V vote, string decisionId)
        {
            Sender = sender;
            Vote = vote;
            DecisionId = decisionId;
        }
    }
}
