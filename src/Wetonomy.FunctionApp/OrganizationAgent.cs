using Apocryph.Agents.Testbed;
using Apocryph.Agents.Testbed.Api;
using Microsoft.Azure.WebJobs;
using Perper.WebJobs.Extensions.Config;
using Perper.WebJobs.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wetonomy.TokenActionAgents.Functions;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using static Wetonomy.Program;

namespace Wetonomy.FunctionApp
{
    public class InitMessage { }

    public class OrganizationAgent
    {
        public class OrganizationState
        {
            public Dictionary<string, AgentCapability> MoneyTokenMinterCapabilities;
        }
        public Task<AgentContext<OrganizationState>> Run(object state, AgentCapability self, object message)
        {
            var organizationState = state as OrganizationState?? new OrganizationState();
            var context = new AgentContext<OrganizationState>(organizationState, self);
            var distributeCapability = context.IssueCapability(new[] { typeof(DistributeCapabilitiesMessage) });

            switch (message)
            {
                case AgentRootInitMessage _:
                    var moneyTokenManagerInitMessage = new TokenManagerInitMessage
                    {
                        CreatorAgentCapability = distributeCapability
                    };

                    context.CreateAgent("moneyTokenManager", "TokenManagerAgent", moneyTokenManagerInitMessage, null);

                    

                    break;

                case DistributeCapabilitiesMessage DistributeCapabilitiesMessage:
                    switch (DistributeCapabilitiesMessage.Id)
                    {
                        case "moneyTokenManager":
                            var mintCapability = DistributeCapabilitiesMessage.AgentCapabilities["MintTokenMessage"];
                            var moneyTokenMinter = new TokenActionAgentInitMessage<AgentCapability>(
                                mintCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), TriggeredAction<AgentCapability>>());
                            context.CreateAgent("moneyTokenMinter", "TokenMinterAgent", moneyTokenMinter, null);
                            break;

                        case "moneyTokenMinter":
                            context.State.MoneyTokenMinterCapabilities = DistributeCapabilitiesMessage.AgentCapabilities;

                            var cashTokenManagerInitMessage = new TokenManagerInitMessage
                            {
                                CreatorAgentCapability = distributeCapability
                            };
                            var debtTokenManagerInitMessage = new TokenManagerInitMessage
                            {
                                CreatorAgentCapability = distributeCapability
                            };
                            var allowanceTokenManagerInitMessage = new TokenManagerInitMessage
                            {
                                CreatorAgentCapability = distributeCapability
                            };

                            context.CreateAgent("cashTokenManager", "TokenManagerAgent", cashTokenManagerInitMessage, null);
                            //context.CreateAgent("debtTokenManager", "TokenManagerAgent", debtTokenManagerInitMessage, null);
                            //context.CreateAgent("allowanceTokenBurner", "TokenManagerAgent", allowanceTokenManagerInitMessage, null);
                            break;

                        case "cashTokenManager":
                            //var splitCapability = DistributeCapabilitiesMessage.AgentCapabilities["TransferTokenMessage"];
                            //var tokenSplitterAgent = new TokenActionAgentInitMessage<AgentCapability>(
                            //    splitCapability,
                            //    distributeCapability,
                            //    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                            //    {
                            //            { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenSplitterFunctions<AgentCapability>.UniformSplitter}
                            //    });
                            //context.CreateAgent("cashTokenSplitter", "TokenSplitterAgent", tokenSplitterAgent, null);


                            var burnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var cashTokenBurnerForDebt = new TokenActionAgentInitMessage<AgentCapability>(
                                burnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SelfBurn}
                                });
                            context.CreateAgent("cashTokenBurnerForDebt", "TokenBurnerAgent", cashTokenBurnerForDebt, null);


                            //var cashTokenBurnerForAllowance = new TokenActionAgentInitMessage<AgentCapability>(
                            //    burnCapability,
                            //    distributeCapability,
                            //    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                            //    {
                            //            { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SelfBurn}
                            //    });
                            //context.CreateAgent("cashTokenBurnerForAllowance", "TokenBurnerAgent", cashTokenBurnerForDebt, null);
                            break;

                        case "debtTokenManager":
                            //Need subscribtion
                            var debtBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var debtTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                debtBurnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenBurnerForDebt" ,typeof(TokensBurnedTriggerer<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SequentialBurn}
                                });
                            context.CreateAgent("debtTokenBurner", "TokenBurnerAgent", debtTokenBurner, null);
                            break;

                        case "allowanceTokenManager":
                            //Need subscribtion
                            var allowanceBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var allowanceTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                allowanceBurnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenBurnerForAllowance" ,typeof(TokensBurnedTriggerer<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SequentialBurn}
                                });
                            context.CreateAgent("allowanceTokenBurner", "TokenBurnerAgent", allowanceTokenBurner, null);
                            break;

                        case "debtTokenBurner":
                            //Need subscribtion
                            var cap1 = context.State.MoneyTokenMinterCapabilities["AddTriggerToActionMessage"];
                            var debtBurnTrigger = new AddTriggerToActionMessage<AgentCapability>(("debtTokenBurner", typeof(TokensBurnedTriggerer<AgentCapability>)), TokenMinterFunctions<AgentCapability>.SingleMintAfterBurn);
                            context.SendMessage(cap1, debtBurnTrigger, null);
                            break;

                        case "allowanceTokenBurner":
                            //Need subscribtion
                            var cap2 = context.State.MoneyTokenMinterCapabilities["AddTriggerToActionMessage"];
                            var allowanceBurnTrigger = new AddTriggerToActionMessage<AgentCapability>(("allowanceTokenBurner", typeof(TokensBurnedTriggerer<AgentCapability>)), TokenMinterFunctions<AgentCapability>.SingleMintAfterBurn);
                            context.SendMessage(cap2, allowanceBurnTrigger, null);
                            break;
                    }
                    break;

            }
            return Task.FromResult(context);
        }
    }

    public class OrganizationWrapper
    {
        private readonly Testbed _testbed;

        public OrganizationWrapper(Testbed testbed)
        {
            _testbed = testbed;
        }

        [FunctionName("OrganizationAgent")]
        public async Task OrganizationAgent(
            [PerperStreamTrigger] PerperStreamContext context,
            [Perper("agentId")] string agentId,
            [Perper("initMessage")] object initMessage,
            [PerperStream("commands")] IAsyncEnumerable<AgentCommands> commands,
            [PerperStream("output")] IAsyncCollector<AgentCommands> output,
            CancellationToken cancellationToken)
        {
            await _testbed.Agent(new OrganizationAgent().Run, agentId, initMessage, commands, output, cancellationToken);
        }
    }
}
