using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenActionAgents.Messages
{
    class RemoveRecipientMessage<T>
    {
        public T Recipient;

        public RemoveRecipientMessage(T recipient)
        {
            Recipient = recipient;
        }
    }
}
