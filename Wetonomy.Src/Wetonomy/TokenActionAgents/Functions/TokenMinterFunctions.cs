using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Functions
{
    
    public static class TokenMinterFunctions<T> where T : IEquatable<T>
    {
        public static IList<object> SingleMintAfterBurn(RecipientState<T> _, AbstractTriggerer message)
        {
            if(message is TokensBurnedTriggerer<T> msg)
            {
                var command = new MintTokenMessage<T>(msg.Amount, msg.From);
                return new List<object>() { command };
            }
            return null;
        }
    }
}
