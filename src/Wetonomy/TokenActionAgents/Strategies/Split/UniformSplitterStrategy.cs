using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies.Split
{
	public class UniformSplitterStrategy<T> : ITriggeredAction<T> where T : IEquatable<T>
	{
		public IList<object> Execute(State.RecipientState<T> state, AbstractTrigger message)
		{
            var result = new List<object>();
            BigInteger amount = message.Amount;
            int count = state.Recipients.Count;
            BigInteger portion = amount / count;
            // We are going to lose tokens because we are using integer
            foreach (T recipient in state.Recipients)
            {
                var command = new TransferTokenMessage<T>(portion, default, recipient);
                amount -= portion;
                result.Add(command);
            }
            return result;
        }
	}
}
