using Apocryph.FunctionApp.Agent;
using System;
using System.Collections.Generic;
using Wetonomy.TokenActionAgents;
using Wetonomy.TokenActionAgents.Functions;
using Wetonomy.TokenActionAgents.Messages;
using Wetonomy.TokenActionAgents.Messages.Notifications;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;

namespace Wetonomy
{
    public class Program
    {
        public class DistributeTokenManagerCapabilitiesMessage
        {
            public string Id { get; set; }
            public Dictionary<string, AgentCapability> TokenManagerCapabilities { get; set; }
        }

        public class OrganizationAgent
        {
            public static AgentContext<object> Run(object state, AgentCapability sender, object message)
            {
                var context = new AgentContext<object>(null);

                switch (message)
                {
                    case InitMessage _:
                        var distributeCapability = context.IssueCapability(new[]
                            {
                                nameof(DistributeTokenManagerCapabilitiesMessage)
                            });
                        var cashTokenManagerInitMessage = new TokenManagerInitMessage
                        {
                            Id = "cashTokenManager",
                            OrganizationAgentCapability = distributeCapability
                        };
                        var moneyTokenManagerInitMessage = new TokenManagerInitMessage
                        {
                            Id = "moneyTokenManager",
                            OrganizationAgentCapability = distributeCapability
                        };
                        var debtTokenManagerInitMessage = new TokenManagerInitMessage
                        {
                            Id = "debtTokenManager",
                            OrganizationAgentCapability = distributeCapability
                        };
                        var allowanceTokenManagerInitMessage = new TokenManagerInitMessage
                        {
                            Id = "allowanceTokenBurner",
                            OrganizationAgentCapability = distributeCapability
                        };

                        context.CreateAgent("cashTokenManager", nameof(TokenManager<AgentCapability>), cashTokenManagerInitMessage, null);
                        context.CreateAgent("moneyTokenManager", nameof(TokenManager<AgentCapability>), moneyTokenManagerInitMessage, null);
                        context.CreateAgent("debtTokenManager", nameof(TokenManager<AgentCapability>), debtTokenManagerInitMessage, null);
                        context.CreateAgent("allowanceTokenBurner", nameof(TokenManager<AgentCapability>), allowanceTokenManagerInitMessage, null);

                        break;

                    case DistributeTokenManagerCapabilitiesMessage distributeTokenManagerCapabilitiesMessage:
                        switch (distributeTokenManagerCapabilitiesMessage.Id)
                        {
                            case "cashTokenManager":
                                var splitCapability = distributeTokenManagerCapabilitiesMessage.TokenManagerCapabilities["TransferTokenMessage"];
                                var tokenSplitterAgent = new TokenActionAgentInitMessage<AgentCapability>(
                                    splitCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenSplitterFunctions<AgentCapability>.UniformSplitter}
                                    });
                                context.CreateAgent("cashTokenSplitter", nameof(TokenSplitterAgent<AgentCapability>), tokenSplitterAgent, null);


                                var burnCapability = distributeTokenManagerCapabilitiesMessage.TokenManagerCapabilities["BurnTokenMessage"];
                                var cashTokenBurnerForDebt = new TokenActionAgentInitMessage<AgentCapability>(
                                    burnCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SelfBurn}
                                    });
                                context.CreateAgent("cashTokenBurnerForDebt", nameof(TokenBurnerAgent<AgentCapability>), cashTokenBurnerForDebt, null);


                                var cashTokenBurnerForAllowance = new TokenActionAgentInitMessage<AgentCapability>(
                                    burnCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("cashTokenManager", typeof(TokensTransferedNotification<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SelfBurn}
                                    });
                                context.CreateAgent("cashTokenBurnerForAllowance", nameof(TokenBurnerAgent<AgentCapability>), cashTokenBurnerForDebt, null);
                                break;

                            case "debtTokenManager":
                                var debtBurnCapability = distributeTokenManagerCapabilitiesMessage.TokenManagerCapabilities["BurnTokenMessage"];
                                var debtTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                    debtBurnCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("cashTokenBurnerForDebt" ,typeof(TokensBurnedTriggerer<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SequentialBurn}
                                    });
                                context.CreateAgent("debtTokenBurner", nameof(TokenBurnerAgent<AgentCapability>), debtTokenBurner, null);
                                break;

                            case "allowanceTokenBurner":
                                var allowanceBurnCapability = distributeTokenManagerCapabilitiesMessage.TokenManagerCapabilities["BurnTokenMessage"];
                                var allowanceTokenBurner = new TokenActionAgentInitMessage<AgentCapability>(
                                    allowanceBurnCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("cashTokenBurnerForAllowance" ,typeof(TokensBurnedTriggerer<AgentCapability>)), TokenBurnerFunctions<AgentCapability>.SequentialBurn}
                                    });
                                context.CreateAgent("allowanceTokenBurner", nameof(TokenBurnerAgent<AgentCapability>), allowanceTokenBurner, null);
                                break;

                            case "moneyTokenManager":
                                var mintCapability = distributeTokenManagerCapabilitiesMessage.TokenManagerCapabilities["MintTokenMessage"];
                                var moneyTokenMinter = new TokenActionAgentInitMessage<AgentCapability>(
                                    mintCapability,
                                    new Dictionary<(string, Type), TriggeredAction<AgentCapability>>()
                                    {
                                        { ("moneyTokenManager",typeof(TokensBurnedTriggerer<AgentCapability>)), TokenMinterFunctions<AgentCapability>.SingleMintAfterBurn}
                                    });
                                context.CreateAgent("moneyTokenMinter", nameof(TokenMinterAgent<AgentCapability>), moneyTokenMinter, null);
                                break;
                        }
                        break;

                }
                return context;
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
