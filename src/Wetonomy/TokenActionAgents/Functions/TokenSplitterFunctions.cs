using System;
using System.Collections.Generic;
using System.Numerics;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Functions
{
    public static class TokenSplitterFunctions<T> where T : IEquatable<T>
    {
        public static IList<object> UniformSplitter(RecipientState<T> state, AbstractTrigger message)
        {
            var result = new List<object>();
            BigInteger amount = message.Amount;
            int count = state.Recipients.Count;
            BigInteger portion = amount / count;
            // We are count to lose tokens because we are using integer
            foreach (T recipient in state.Recipients)
            {
                var command = new TransferTokenMessage<T>(portion, default, recipient);
                amount -= portion;
                result.Add(command);
            }
            return result;
        }
    }
    interface ITokenPair<T>
    {
        T GetTag();
        T GetAgentId();
    }

    class TokenPair : ITokenPair<string>
    {
        public string GetAgentId()
        {
            throw new NotImplementedException();
        }

        public string GetTag()
        {
            throw new NotImplementedException();
        }
    }
}
