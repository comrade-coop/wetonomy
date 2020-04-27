using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.TokenActionAgents.Strategies.Split
{
	public class UniformSplitterStrategy: ITriggeredAction
	{
        public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message)
        {
            var messagesResult = new List<object>();
            if (message is TokensTransferedNotification msg && msg.To.GetAgentId().Equals(state.SelfId)) {
                BigInteger amount = message.Amount;
                int count = state.Recipients.Count;
                if (count == 0) throw new Exception();
                BigInteger portion = amount / count;
                // We are going to lose tokens because we are using integer
                foreach (IAgentTokenKey recipient in state.Recipients)
                {
                    //msg.To.ChangeAgentId(state.SelfId)
                    var command = new TransferTokenMessage(portion, new SingleAngentTokenKey(state.SelfId), recipient);
                    amount -= portion;
                    messagesResult.Add(command);
                }
            }
            return (messagesResult, null);
        }
	}
}
