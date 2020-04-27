using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies
{
	public interface ITriggeredAction
	{
		//return 2 lists - one with messages and with publications
		public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message);
    }
}
