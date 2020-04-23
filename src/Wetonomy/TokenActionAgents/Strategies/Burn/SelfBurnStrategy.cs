using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Burn
{
	public class SelfBurnStrategy : ITriggeredAction
	{
		public IList<object> Execute(RecipientState _, AbstractTrigger message)
		{
			if (message is TokensTransferedNotification msg)
			{
				var command = new BurnTokenMessage(msg.Amount, msg.To);
				return new List<object>() { command };
			}
			return null;
		}
	}
}
