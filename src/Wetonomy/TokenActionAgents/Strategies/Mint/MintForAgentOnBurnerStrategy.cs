using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies.Mint
{
	//Tokens are minted on burner's addres so that he can burn them, 
	//but they are actually for the user TokensMintedTriggerer.To
	//He is added as recipient and he can take ownership of the tokens with GetTokensMessage
	//This is done so that the system can work automaticly
	public class MintForAgentOnBurnerStrategy : ITriggeredAction
	{
		public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message)
		{
			if (message is MockMessageTrigger msg)
			{
				var command = new MintTokenMessage(msg.Amount, msg.To.ChangeAgentId(state.SelfId));

				var publication = new TokensMintedTriggerer(state.SelfId, msg.Amount, msg.To);

				return (new List<object>() { command }, new List<object>() { publication });
			}
			return (null, null);
		}
	}

	public class MockMessageTrigger : AbstractTrigger
	{
		public IAgentTokenKey To { get; }

		public MockMessageTrigger(string sender, BigInteger amount, IAgentTokenKey to) : base(amount, sender)
		{
			To = to;
		}
	}
}
