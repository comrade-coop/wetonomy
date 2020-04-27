using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Messages
{
    public class FinalizeDecision
    {
        public string DecisionId { get; }

        public FinalizeDecision(string id)
        {
            DecisionId = id;
        }
    }
}
