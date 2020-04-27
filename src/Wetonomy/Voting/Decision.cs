using Apocryph.Agents.Testbed.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting
{
    public enum DecisionState
    {
        Active,
        Finalized
    }
    public class Decision<T>
    {
        public string DecisionId { get; }
        public ForwardableMessage DecisionActionMessage { get; }

        public bool Executable { get; }

        public DateTime Start { get; }

        public DateTime Finale { get; }

        public DecisionState State { get; set; }

        public T Evaluation { get; set; }

        public Decision(string id, bool executable, ForwardableMessage message, DateTime start, DateTime finale)
        {
            DecisionId = id;
            Executable = executable;
            DecisionActionMessage = message;
            Start = start;
            Finale = finale;
            State = DecisionState.Active;
        }
    }
}
