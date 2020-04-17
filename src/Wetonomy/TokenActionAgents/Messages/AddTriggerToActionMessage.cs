using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Strategies;

namespace Wetonomy.TokenActionAgents.Messages
{
	public class AddTriggerToActionMessage<T> where T: IEquatable<T>
	{
		public (string, Type) Trigger { get; }
		public ITriggeredAction<T> Action { get; }

		public AddTriggerToActionMessage((string, Type) trigger, ITriggeredAction<T> action)
		{
			Trigger= trigger;
			Action = action;
		}
	}
}
