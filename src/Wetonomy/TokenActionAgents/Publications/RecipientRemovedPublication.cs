using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenActionAgents.Publications
{
    class RecipientRemovedPublication<T>
    {
        public T Recipient;

        public RecipientRemovedPublication(T recipient)
        {
            Recipient = recipient;
        }
    }
}
