using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Burn
{
	public class SelfBurnStrategy<T> : ITriggeredAction<T> where T: IEquatable<T>
	{
		public IList<object> Execute(RecipientState<T> _, AbstractTrigger message)
		{
			if (message is TokensTransferedNotification<T> msg)
			{
				var command = new BurnTokenMessage<T>(msg.Amount, msg.To);
				return new List<object>() { command };
			}
			return null;
		}
	}
}
