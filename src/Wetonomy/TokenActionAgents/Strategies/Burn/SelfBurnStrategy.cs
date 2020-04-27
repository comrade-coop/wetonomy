using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Burn
{
	public class SelfBurnStrategy : ITriggeredAction
	{
		public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message)
		{
			//here To is slef
			if (message is TokensTransferedNotification msg)
			{
				var command = new BurnTokenMessage(msg.Amount, msg.To);
				var publication = new TokensBurnedTriggerer(state.SelfId, msg.Amount, msg.To);
				return (new List<object>() { command }, new List<object>() { publication });
			}
			return (null, null);
		}
	}
}
