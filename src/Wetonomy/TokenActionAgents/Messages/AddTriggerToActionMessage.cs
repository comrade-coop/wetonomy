using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;

namespace Wetonomy.TokenActionAgents.Messages
{
	public class AddTriggerToActionMessage<T> where T: IEquatable<T>
	{
		public (string, Type) Trigger { get; }
		public TriggeredAction<T> Action { get; }

		public AddTriggerToActionMessage((string, Type) trigger, TriggeredAction<T> action)
		{
			Trigger= trigger;
			Action = action;
		}
	}
}
