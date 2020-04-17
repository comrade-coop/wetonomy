using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies
{
	public interface ITriggeredAction<T> where T: IEquatable<T>
	{
		public IList<object> Execute(RecipientState<T> state, AbstractTrigger message);
    }
}
