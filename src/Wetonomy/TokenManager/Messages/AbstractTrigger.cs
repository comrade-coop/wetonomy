using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Wetonomy.TokenManager.Messages
{
    //Probaly we should do TokenTrigger which would have IAgentTokenKey instead of string Sender
    public abstract class AbstractTrigger
    {
        public BigInteger Amount { get; set; }

        public string Sender { get; protected set; }

        public AbstractTrigger(BigInteger amount, string sender)
        {
            Amount = amount;
            Sender = sender;
        }
    }
}
