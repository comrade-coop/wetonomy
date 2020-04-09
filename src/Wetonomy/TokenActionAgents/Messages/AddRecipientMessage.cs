using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenActionAgents.Messages
{
    public class AddRecipientMessage<T>
    {
        public T Recipient;

        public AddRecipientMessage(T recipient)
        {
            Recipient = recipient;
        }
    }
}
