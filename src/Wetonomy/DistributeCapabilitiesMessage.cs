using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wetonomy
{
    public class DistributeCapabilitiesMessage
    {
        public string Id { get; set; }
        public Dictionary<string, AgentCapability> AgentCapabilities { get; set; }
    }
}
