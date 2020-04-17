using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies.Mint
{
	public class SingleMintAfterBurnStrategy<T> : ITriggeredAction<T> where T : IEquatable<T>
	{
        public IList<object> Execute(RecipientState<T> _, AbstractTrigger message)
        {
            if (message is TokensBurnedTriggerer<T> msg)
            {
                var command = new MintTokenMessage<T>(msg.Amount, msg.From);
                return new List<object>() { command };
            }
            return null;
        }
    }
}
