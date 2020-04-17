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

namespace Wetonomy.FunctionApp
{
	public class ManyAgentsTest
    {
        public Task<AgentContext<object>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<object>(state, self);
            if (message is AgentRootInitMessage rootInitMessage)
            {
                for (int i = 0; i < 10; i++)
                {
                    context.CreateAgent("Agent" + i, "SingleAgent", new NumberMessage() { Creator = self, Number = i }, null);
                }
            }
            else if (message is NotifyNumber msg)
            {
                Console.WriteLine("{0}", msg.Number);
            }
            return Task.FromResult(context);
        }
    }

    public class NumberMessage
    {
        public int Number { get; set; }

        public AgentCapability Creator { get; set; }
    }

    public class NotifyNumber
    {
        public int Number { get; set; }
    }

    public class AgentOneWrapper
    {
        private readonly Testbed _testbed;

        public AgentOneWrapper(Testbed testbed)
        {
            _testbed = testbed;
        }

        [FunctionName("ManyAgentsTest")]
        public async Task AgentOne(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new ManyAgentsTest().Run, agentId, initMessage, commands, output, cancellationToken);
        }
    }
}
