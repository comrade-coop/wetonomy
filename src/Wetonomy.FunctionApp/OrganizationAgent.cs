using Apocryph.Agents.Testbed;
using Apocryph.Agents.Testbed.Api;
using Microsoft.Azure.WebJobs;
using Perper.WebJobs.Extensions.Config;
using Perper.WebJobs.Extensions.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.Strategies;
using Wetonomy.TokenActionAgents.Strategies.Burn;
using Wetonomy.TokenActionAgents.Strategies.Mint;
using Wetonomy.TokenActionAgents.Strategies.Split;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy.FunctionApp
{
    public class OrganizationAgent
    {
        public class OrganizationState
        {
            public Dictionary<string, Dictionary<string, AgentCapability>> AgentToCapabilities;

            public OrganizationState()
            {
                AgentToCapabilities = new Dictionary<string, Dictionary<string, AgentCapability>>();
            }
        }
        public Task<AgentContext<OrganizationState>> Run(object state, AgentCapability self, object message)
        {
            var organizationState = state as OrganizationState?? new OrganizationState();
            var context = new AgentContext<OrganizationState>(organizationState, self);
            var distributeCapability = context.IssueCapability(new[] { typeof(DistributeCapabilitiesMessage) });

            switch (message)
            {
                case AgentRootInitMessage _:

                    var memberFactoryInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                    context.CreateAgent("memberAndGroupFactory", "MemberAndGroupFactoryAgent", memberFactoryInitMessage, null);


                    
                    break;

                case DistributeCapabilitiesMessage DistributeCapabilitiesMessage:
                    context.State.AgentToCapabilities.Add(DistributeCapabilitiesMessage.Id, DistributeCapabilitiesMessage.AgentCapabilities);

                    switch (DistributeCapabilitiesMessage.Id)
                    {

                        // -------- Members, Groups & Voting -------- 
                        case "memberAndGroupFactory":
                            //var createFirstMemberMsg = new CreateGroupMessage("FirstGroup");
                            //var creatGroupMessage = DistributeCapabilitiesMessage.AgentCapabilities["MintTokenMessage"];
                            //context.SendMessage(creatGroupMessage, createFirstMemberMsg, null);

                            var votingInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                            context.CreateAgent("centralVoting", "VotingAgent", votingInitMessage, null);
                            break;

                        case "centralVoting":
                            //var votingInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                            //context.CreateAgent("centralVoting", "VotingAgent", votingInitMessage, null);
                            var moneyTokenManagerInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                            context.CreateAgent("moneyTokenManager", "TokenManagerAgent", moneyTokenManagerInitMessage, null);
                            break;

                        // -------- Token Flow -------- 

                        case "moneyTokenManager":
                            var mintCapability = DistributeCapabilitiesMessage.AgentCapabilities["MintTokenMessage"];
                            var moneyTokenMinter = new TokenActionAgentInitMessage<AgentCapability>(
                                mintCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>(),
                                new HashSet<string>() { "debtTokenBurner", "allowanceTokenBurner" });
                            context.CreateAgent("moneyTokenMinter", "TokenMinterAgent", moneyTokenMinter, null);
                            break;

                        case "moneyTokenMinter":

                            var cashTokenManagerInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                            var debtTokenManagerInitMessage = new InitWetonomyAgentMessage(distributeCapability);
                            var allowanceTokenManagerInitMessage = new InitWetonomyAgentMessage(distributeCapability);

                            context.CreateAgent("cashTokenManager", "TokenManagerAgent", cashTokenManagerInitMessage, null);
                            context.CreateAgent("debtTokenManager", "TokenManagerAgent", debtTokenManagerInitMessage, null);
                            context.CreateAgent("allowanceTokenManager", "TokenManagerAgent", allowanceTokenManagerInitMessage, null);
                            break;

                        case "cashTokenManager":
                            var splitCapability = DistributeCapabilitiesMessage.AgentCapabilities["TransferTokenMessage"];
                            var tokenSplitterAgent = new TokenActionAgentInitMessage<AgentCapability>(
                                splitCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), new UniformSplitterStrategy<AgentCapability>()}
                                },
                                null);
                            context.CreateAgent("cashTokenSplitter", "TokenSplitterAgent", tokenSplitterAgent, null);


                            var burnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var cashTokenBurnerForDebt = new TokenActionAgentInitMessage<AgentCapability>(
                                burnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), new SelfBurnStrategy<AgentCapability>()}
                                },
                                null);
                            context.CreateAgent("cashTokenBurnerForDebt", "TokenBurnerAgent", cashTokenBurnerForDebt, null);


                            var cashTokenBurnerForAllowance = new TokenActionAgentInitMessage<AgentCapability>(
                                burnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), new SelfBurnStrategy<AgentCapability>()}
                                },
                                null);
                            context.CreateAgent("cashTokenBurnerForAllowance", "TokenBurnerAgent", cashTokenBurnerForAllowance, null);
                            break;

                        case "debtTokenManager":
                            //Need subscribtion
                            var debtBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var debtTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                debtBurnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenBurnerForDebt" ,typeof(TokensBurnedTriggerer<AgentCapability>)), new SequentialBurnStrategy<AgentCapability>()}
                                },
                                new HashSet<string>() { "cashTokenBurnerForDebt" });
                            context.CreateAgent("debtTokenBurner", "TokenBurnerAgent", debtTokenBurner, null);
                            break;

                        case "allowanceTokenManager":
                            //Need subscribtion
                            var allowanceBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var allowanceTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                allowanceBurnCapability,
                                distributeCapability,
                                new Dictionary<(string, Type), ITriggeredAction<AgentCapability>>()
                                {
                                        { ("cashTokenBurnerForAllowance" ,typeof(TokensBurnedTriggerer<AgentCapability>)), new SequentialBurnStrategy<AgentCapability>()}
                                },
                                new HashSet<string>() { "cashTokenBurnerForAllowance" });
                            context.CreateAgent("allowanceTokenBurner", "TokenBurnerAgent", allowanceTokenBurner, null);
                            break;

                        case "debtTokenBurner":
                            //Need subscribtion
                            var cap1 = context.State.AgentToCapabilities["moneyTokenMinter"]["AddTriggerToActionMessage"];
                            var debtBurnTrigger = new AddTriggerToActionMessage<AgentCapability>(("debtTokenBurner", typeof(TokensBurnedTriggerer<AgentCapability>)), new SingleMintAfterBurnStrategy<AgentCapability>());
                            context.SendMessage(cap1, debtBurnTrigger, null);
                            break;

                        case "allowanceTokenBurner":
                            //Need subscribtion
                            var cap2 = context.State.AgentToCapabilities["moneyTokenMinter"]["AddTriggerToActionMessage"];
                            var allowanceBurnTrigger = new AddTriggerToActionMessage<AgentCapability>(("allowanceTokenBurner", typeof(TokensBurnedTriggerer<AgentCapability>)), new SingleMintAfterBurnStrategy<AgentCapability>());
                            context.SendMessage(cap2, allowanceBurnTrigger, null);

                            // Imitate user action
                            var mintCap = context.IssueCapability(new[] { typeof(TokensMintedNotification<AgentCapability>) });
                            context.AddReminder(TimeSpan.FromSeconds(20), new MintTokenMessage<AgentCapability>(100, mintCap));
                            break;
                    }
                    break;

                case MintTokenMessage<AgentCapability> mintCashToken:
                    var cashTokenManager = context.State.AgentToCapabilities["cashTokenManager"]["MintTokenMessage"];
                    context.SendMessage(cashTokenManager, mintCashToken, null);
                    break;

                case TokensMintedNotification<AgentCapability> tokensMintedNotification:
                    var transferCashTokenManager = context.State.AgentToCapabilities["cashTokenManager"]["TransferTokenMessage"];
                    context.SendMessage(transferCashTokenManager, new TransferTokenMessage<AgentCapability>(100,), null);
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
