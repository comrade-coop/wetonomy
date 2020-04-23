using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies
{
	public interface ITriggeredAction
	{
		public IList<object> Execute(RecipientState state, AbstractTrigger message);
    }
}
