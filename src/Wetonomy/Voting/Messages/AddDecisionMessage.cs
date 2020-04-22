using Apocryph.Agents.Testbed.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Messages
{
    public class AddDecisionMessage
    {
        public bool Executable { get; }
        public ForwardableMessage ActionMessage { get; }
        public DateTime Start { get; }
        public DateTime Finale { get; }

        public AddDecisionMessage(ForwardableMessage message, bool executable, DateTime start, DateTime finale)
        {
            Executable = executable;
            ActionMessage = message;
            Start = start;
            Finale = finale;
        }
    }
}
