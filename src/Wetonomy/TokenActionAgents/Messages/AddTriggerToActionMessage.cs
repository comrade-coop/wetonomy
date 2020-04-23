using System;
using System.Collections.Generic;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenActionAgents.Strategies;

namespace Wetonomy.TokenActionAgents.Messages
{
	public class AddTriggerToActionMessage
	{
		public AgentTriggerPair Trigger { get; }
		public ITriggeredAction Action { get; }

		public AddTriggerToActionMessage(AgentTriggerPair trigger, ITriggeredAction action)
		{
			Trigger= trigger;
			Action = action;
		}
	}
}
