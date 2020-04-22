﻿using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy
{
	public class InitWetonomyAgentMessage
	{
		public AgentCapability CreatorAgentCapability { get; set; }

		public InitWetonomyAgentMessage(AgentCapability creatorAgentCapability)
			=> CreatorAgentCapability = creatorAgentCapability;
	}
}
