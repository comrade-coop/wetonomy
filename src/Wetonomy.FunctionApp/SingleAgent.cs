using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apocryph.Agents.Testbed;
using Apocryph.Agents.Testbed.Api;
using Microsoft.Azure.WebJobs;
using Perper.WebJobs.Extensions.Config;
using Perper.WebJobs.Extensions.Model;

namespace Wetonomy.FunctionApp
{
    public class SingleAgent
    {
        public Task<AgentContext<object>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<object>(state, self);
            if (message is NumberMessage msg)
            {
                context.SendMessage(msg.Creator, new NotifyNumber
                {
                    Number = msg.Number
                }, null);
            }
                
            
            return Task.FromResult(context);
        }
    }

    public class SingleAgentWrapper
    {
        private readonly Testbed _testbed;

        public SingleAgentWrapper(Testbed testbed)
        {
            _testbed = testbed;
        }

        [FunctionName("SingleAgent")]
        public async Task AgentOne(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new SingleAgent().Run, agentId, initMessage, commands, output, cancellationToken);
        }
    }
}