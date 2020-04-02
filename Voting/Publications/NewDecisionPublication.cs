using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Publications
{
    public class NewDecisionPublication
    {
        public string DecisionId { get; }
        public object DecisionActionMessage { get; }

        public NewDecisionPublication(string id, object message)
        {
            DecisionId = id;
            DecisionActionMessage = message;
        }
    }
}
