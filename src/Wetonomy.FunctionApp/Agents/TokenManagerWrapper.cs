using Apocryph.Agents.Testbed;
using Apocryph.Agents.Testbed.Api;
using Microsoft.Azure.WebJobs;
using Perper.WebJobs.Extensions.Config;
using Perper.WebJobs.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wetonomy.TokenManager;
using Wetonomy.TokenActionAgents;
using Wetonomy.TokenActionAgents.State;

namespace Wetonomy.FunctionApp.Agents
{
    public class TokenManagerWrapper
    {
        private readonly Testbed _testbed;

        public TokenManagerWrapper(Testbed testbed)
        {
            _testbed = testbed;
        }

        [FunctionName("TokenManagerAgent")]
        public async Task TokenManagerAgent(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new TokenManagerAgent<AgentCapability>().Run, agentId, initMessage, commands, output, cancellationToken);
        }

        [FunctionName("TokenSplitterAgent")]
        public async Task TokenSplitterAgent(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new TokenSplitterAgent<AgentCapability>().Run, agentId, initMessage, commands, output, cancellationToken);
        }

        [FunctionName("TokenBurnerAgent")]
        public async Task TokenBurnerAgent(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new TokenBurnerAgent<AgentCapability>().Run, agentId, initMessage, commands, output, cancellationToken);
        }

        [FunctionName("TokenMinterAgent")]
        public async Task TokenMinterAgent(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new TokenMinterAgent<AgentCapability>().Run, agentId, initMessage, commands, output, cancellationToken);
        }
    }
}
