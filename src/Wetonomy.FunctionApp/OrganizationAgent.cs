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
using Wetonomy.TokenActionAgents.State;
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
                            var moneyTokenMinter = new TokenActionAgentInitMessage(
                                mintCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair, ITriggeredAction>(),
                                null,
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
                            var tokenSplitterAgent = new TokenActionAgentInitMessage(
                                splitCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair, ITriggeredAction>()
                                {
                                        { new AgentTriggerPair("cashTokenManager", typeof(TokensTransferedNotification)), new UniformSplitterStrategy()}
                                },
                                new List<IAgentTokenKey>() { new SingleAngentTokenKey("cashTokenBurnerForDebt"), new SingleAngentTokenKey("cashTokenBurnerForAllowance") });
                            context.CreateAgent("cashTokenSplitter", "TokenSplitterAgent", tokenSplitterAgent, null);


                            var burnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var cashTokenBurnerForDebt = new TokenActionAgentInitMessage(
                                burnCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair,ITriggeredAction>()
                                {
                                        { new AgentTriggerPair("cashTokenManager", typeof(TokensTransferedNotification)), new SelfBurnStrategy()}
                                });
                            context.CreateAgent("cashTokenBurnerForDebt", "TokenBurnerAgent", cashTokenBurnerForDebt, null);


                            var cashTokenBurnerForAllowance = new TokenActionAgentInitMessage(
                                burnCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair,ITriggeredAction>()
                                {
                                        { new AgentTriggerPair("cashTokenManager", typeof(TokensTransferedNotification)), new SelfBurnStrategy()}
                                });
                            context.CreateAgent("cashTokenBurnerForAllowance", "TokenBurnerAgent", cashTokenBurnerForAllowance, null);
                            break;

                        case "debtTokenManager":
                            //Need subscribtion
                            var debtBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var debtTokenBurner = new TokenActionAgentInitMessage(
                                debtBurnCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair,ITriggeredAction>()
                                {
                                        { new AgentTriggerPair("cashTokenBurnerForDebt", typeof(TokensBurnedTriggerer)), new SequentialBurnStrategy()}
                                },
                                null,
                                new HashSet<string>() { "cashTokenBurnerForDebt" });
                            context.CreateAgent("debtTokenBurner", "TokenBurnerAgent", debtTokenBurner, null);
                            break;

                        case "allowanceTokenManager":
                            //Need subscribtion
                            var allowanceBurnCapability = DistributeCapabilitiesMessage.AgentCapabilities["BurnTokenMessage"];
                            var allowanceTokenBurner = new TokenActionAgentInitMessage(
                                allowanceBurnCapability,
                                distributeCapability,
                                new Dictionary<AgentTriggerPair,ITriggeredAction>()
                                {
                                        { new AgentTriggerPair("cashTokenBurnerForAllowance" ,typeof(TokensBurnedTriggerer)), new SequentialBurnStrategy()}
                                },
                                null,
                                new HashSet<string>() { "cashTokenBurnerForAllowance" });
                            context.CreateAgent("allowanceTokenBurner", "TokenBurnerAgent", allowanceTokenBurner, null);
                            break;

                        case "debtTokenBurner":
                            //Need subscribtion
                            var cap1 = context.State.AgentToCapabilities["moneyTokenMinter"]["AddTriggerToActionMessage"];
                            var debtBurnTrigger = new AddTriggerToActionMessage(new AgentTriggerPair("debtTokenBurner", typeof(TokensBurnedTriggerer)), new SingleMintAfterBurnStrategy());
                            context.SendMessage(cap1, debtBurnTrigger, null);
                            break;

                        case "allowanceTokenBurner":
                            //Need subscribtion
                            var cap2 = context.State.AgentToCapabilities["moneyTokenMinter"]["AddTriggerToActionMessage"];
                            var allowanceBurnTrigger = new AddTriggerToActionMessage(new AgentTriggerPair("allowanceTokenBurner", typeof(TokensBurnedTriggerer)), new SingleMintAfterBurnStrategy());
                            context.SendMessage(cap2, allowanceBurnTrigger, null);

                            // Imitate user action
                            var mintSingleKey = new SingleAngentTokenKey(self.Issuer);
                            context.AddReminder(TimeSpan.FromSeconds(5), new MintTokenMessage(100, mintSingleKey));
                            break;
                    }
                    break;

                case MintTokenMessage mintCashToken:
                    var cashTokenManager = context.State.AgentToCapabilities["cashTokenManager"]["MintTokenMessage"];
                    context.SendMessage(cashTokenManager, mintCashToken, null);
                    break;

                case TokensMintedNotification tokensMintedNotification:
                    var transferCashTokenManager = context.State.AgentToCapabilities["cashTokenManager"]["TransferTokenMessage"];
                    context.SendMessage(transferCashTokenManager, new TransferTokenMessage(100, new SingleAngentTokenKey(self.Issuer), new SingleAngentTokenKey("cashTokenSplitter")), null);
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
