using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Agents.Testbed;
using Apocryph.Agents.Testbed.Api;
using Microsoft.Azure.WebJobs;
using Perper.WebJobs.Extensions.Config;
using Perper.WebJobs.Extensions.Model;

namespace Apocryph.Agent.FunctionApp.Agents
{
    public class AgentOne
    {
        public Task<AgentContext<object>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<object>(state, self);
            if (message is AgentRootInitMessage rootInitMessage)
            {
                var cap = context.IssueCapability(new[] {typeof(PingPongMessage)});
                context.CreateAgent("AgentTwoId", "AgentTwo", new PingPongMessage {AgentOne = cap}, null);
            }
            else if(message is PingPongMessage pingPongMessage)
            {
                context.SendMessage(pingPongMessage.AgentTwo, new PingPongMessage
                {
                    AgentOne = pingPongMessage.AgentOne,
                    AgentTwo = pingPongMessage.AgentTwo,
                    Content = "Ping"
                }, null);
            }
            return Task.FromResult(context);
        }
    }

    public class AgentOneWrapper
    {
        private readonly Testbed _testbed;

        public AgentOneWrapper(Testbed testbed)
        {
            _testbed = testbed;
        }

        [FunctionName("AgentOne")]
        public async Task AgentOne(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new AgentOne().Run, agentId, initMessage, commands, output, cancellationToken);
        }
    }
}