using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies.Mint
{
	public class SingleMintAfterBurnStrategy : ITriggeredAction
	{
        public IList<object> Execute(RecipientState _, AbstractTrigger message)
        {
            if (message is TokensBurnedTriggerer msg)
            {
                var command = new MintTokenMessage(msg.Amount, msg.From);
                return new List<object>() { command };
            }
            return null;
        }
    }
}
