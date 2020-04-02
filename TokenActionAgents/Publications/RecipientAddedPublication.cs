using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy.TokenActionAgents.Publications
{
    class RecipientAddedPublication<T>
    {
        public T Recipient;

        public RecipientAddedPublication(T recipient)
        {
            Recipient = recipient;
        }
    }
}
