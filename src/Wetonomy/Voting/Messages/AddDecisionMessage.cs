using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.Voting.Messages
{
    public class AddDecisionMessage
    {
        public bool Executable { get; }
        public object ActionMessage { get; }
        public DateTime Start { get; }
        public DateTime Finale { get; }

        public AddDecisionMessage(object message, bool executable, DateTime start, DateTime finale)
        {
            Executable = executable;
            ActionMessage = message;
            Start = start;
            Finale = finale;
        }
    }
}
