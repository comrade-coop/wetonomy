using Apocryph.Agents.Testbed.Api;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Wetonomy.TokenManager.Messages;
using Wetonomy.TokenManager.Messages.NotificationsMessages;
using Wetonomy.TokenManager.Publications;

namespace Wetonomy.TokenManager
{
    public class TokenManagerAgent
    {
        public class TokenManagerState: ITokenManagerState
        {
            public BigInteger TotalBalance { get; private set; }
            public Dictionary<IAgentTokenKey, BigInteger> TokenBalances = new Dictionary<IAgentTokenKey, BigInteger>();


            public bool Burn(BigInteger amount, IAgentTokenKey from)
            {
                if (!TokenBalances.ContainsKey(from)) return false;
                BigInteger current = TokenBalances[from];
                if(current > amount)
                {
                    TokenBalances[from] -= amount;
                    TotalBalance-=amount;
                    return true;
                }
                if(current == amount)
                {
                    TotalBalance -= amount;
                    TokenBalances.Remove(from);
                    return true;
                }
                return false;
            }

            public bool Mint(BigInteger amount, IAgentTokenKey to)
            {
                if (TokenBalances.ContainsKey(to)) TokenBalances[to] += amount;

                else TokenBalances.Add(to, amount);

                TotalBalance += amount;
                return true;
            }

            public bool Transfer(BigInteger amount, IAgentTokenKey from, IAgentTokenKey to)
            {
                if (TokenBalances.ContainsKey(from))
                {
                    BigInteger current = TokenBalances[from];
                    if (!TokenBalances.ContainsKey(to)) TokenBalances.Add(to, 0);
                    if (current > amount)
                    {
                        TokenBalances[from] -= amount;
                        TokenBalances[to] += amount;
                        return true;
                    }
                    if (current == amount)
                    {
                        TokenBalances.Remove(from);
                        TokenBalances[to] += amount;
                        return true;
                    }
                }
                throw new Exception();
            }
        }

        public Task<AgentContext<TokenManagerState>> Run(object state, AgentCapability self, object message)
        {
            var agentState = state as TokenManagerState ?? new TokenManagerState();
            var context = new AgentContext<TokenManagerState>(agentState, self);
            switch (message)
            {
                case InitWetonomyAgentMessage tokenManagerInitMessage:
                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"BurnTokenMessage", context.IssueCapability(new[]{ typeof(BurnTokenMessage) }) },
                            {"MintTokenMessage", context.IssueCapability(new[]{ typeof(MintTokenMessage) }) },
                            {"TransferTokenMessage", context.IssueCapability(new[]{ typeof(TransferTokenMessage) }) },
                        }
                    };

                    context.SendMessage(tokenManagerInitMessage.CreatorAgentCapability, distributeCapabilityMessage, null);
                    break;
                case BurnTokenMessage burnTokenMessage:
                    if(context.State.Burn(burnTokenMessage.Amount, burnTokenMessage.From))
                    {
                        var notificationCapability = new AgentCapability(burnTokenMessage.From.GetAgentId(), typeof(TokensMintedNotification));

                        context.SendMessage(notificationCapability, new TokensBurnedNotification(self.Issuer, burnTokenMessage.Amount, burnTokenMessage.From), null);

                        context.MakePublication(
                            new TokenBurnPublication(burnTokenMessage.Amount, burnTokenMessage.From)
                        );
                    }
                    break;

                case MintTokenMessage mintTokenMessage:

                    if (context.State.Mint(mintTokenMessage.Amount, mintTokenMessage.To))
                    {
                        var notificationCapability = new AgentCapability(mintTokenMessage.To.GetAgentId(), typeof(TokensMintedNotification));
                        
                        context.SendMessage(notificationCapability, new TokensMintedNotification(self.Issuer, mintTokenMessage.Amount, mintTokenMessage.To), null);

                        context.MakePublication(
                            new TokenMintPublication(mintTokenMessage.Amount, mintTokenMessage.To)
                        );
                    }
                    break;

                case TransferTokenMessage transferTokenMessage:
                    if (context.State.Transfer(transferTokenMessage.Amount, transferTokenMessage.From, transferTokenMessage.To))
                    {
                        var notificationCapabilityTo = new AgentCapability(transferTokenMessage.To.GetAgentId(), typeof(TokensMintedNotification));
                        var notificationCapabilityFrom = new AgentCapability(transferTokenMessage.From.GetAgentId(), typeof(TokensMintedNotification));

                        context.SendMessage(notificationCapabilityTo, new TokensTransferedNotification(self.Issuer, transferTokenMessage.Amount, transferTokenMessage.From, transferTokenMessage.To), null);
                        context.SendMessage(notificationCapabilityFrom, new TokensTransferedNotification(self.Issuer, transferTokenMessage.Amount, transferTokenMessage.From, transferTokenMessage.To), null);

                        context.MakePublication(
                            new TokenTransferPublication(transferTokenMessage.Amount, transferTokenMessage.From, transferTokenMessage.To)
                        );
                    }
                    break;
            }
            return Task.FromResult(context);
        }
    }
}
