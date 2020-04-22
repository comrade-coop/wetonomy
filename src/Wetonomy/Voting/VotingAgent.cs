using System;
using System.Collections;
using System.Collections.Generic;
using Apocryph.Agents.Testbed.Api;
using Wetonomy.Voting.Messages;
using Wetonomy.Voting.Publications;
using System.Linq;
using Apocryph.Agents.Testbed.Core;
using System.Threading.Tasks;

namespace Wetonomy.Voting
{
    public class VotingAgent<T,V>: ForwardingAgent where V : IEnumerable
    {
        public class VotingState: BaseState
        {
            //Just Temporary solution
            public int nonce = 0;

            public IVoteStategy<T,V> VotingStategy;
            public Dictionary<string, Decision<T>> Decisions = new Dictionary<string, Decision<T>>();
            public Dictionary<string, Dictionary<string, V>> DecisionsVotes = new Dictionary<string, Dictionary<string, V>>();

            //public Dictionary<string, AgentCapability> Capabilities = new Dictionary<string, AgentCapability>();
        }

        public Task<AgentContext<VotingState>> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<VotingState>(state as VotingState, self);

            switch (message)
            {
                case InitWetonomyAgentMessage initMsg:
                    var distributeCapabilityMessage = new DistributeCapabilitiesMessage
                    {
                        Id = self.Issuer,
                        AgentCapabilities = new Dictionary<string, AgentCapability>() {
                            {"AddVoteMessage", context.IssueCapability(new[]{ typeof(AddVoteMessage<T>) })},
                            {"AddDecisionMessage", context.IssueCapability(new[]{ typeof(AddDecisionMessage) })},
                            //{"ForwardMessage", context.IssueCapability(new[]{ typeof(ForwardMessage) })}
                        }
                    };
                    context.SendMessage(initMsg.CreatorAgentCapability, distributeCapabilityMessage, null);
                    break;

                case AddVoteMessage<V> addVoteMessage:
                    context.State.DecisionsVotes[addVoteMessage.DecisionId].Add(addVoteMessage.Sender, addVoteMessage.Vote);
                    context.MakePublication(new NewVotePublication<V>(addVoteMessage.DecisionId, addVoteMessage.Vote));
                    break;

                case AddDecisionMessage addDecisionMessage:
                    var decision = new Decision<T>(
                        context.State.nonce.ToString(),
                        addDecisionMessage.Executable,
                        addDecisionMessage.ActionMessage,
                        addDecisionMessage.Start,
                        addDecisionMessage.Finale);

                    context.State.Decisions.Add(context.State.nonce.ToString(), decision);
                    context.State.DecisionsVotes.Add(context.State.nonce.ToString(), new Dictionary<string, V>());

                    context.MakePublication(
                        new NewDecisionPublication(context.State.nonce.ToString(), addDecisionMessage.ActionMessage)
                    );

                    context.AddReminder(decision.Finale-DateTime.Now, new FinalizeDecision(context.State.nonce.ToString()));
                    //Just Temporary solution
                    context.State.nonce++;
                    break;

                case FinalizeDecision finalizeDecisionMessage:
                    var dec = context.State.Decisions[finalizeDecisionMessage.DecisionId];
                    IEnumerable<V> votes =
                        context.State.DecisionsVotes[finalizeDecisionMessage.DecisionId]
                        .Select(pair => pair.Value);

                    T decisionEvaluation = context.State.VotingStategy.MakeDecision(votes);

                    dec.Evaluation = decisionEvaluation;
                    dec.State = DecisionState.Finalized;


                    if (dec.Executable)
                    {
                        //Executes only if T is bool, not sure if we need execution when T isn't bool
                        if(decisionEvaluation is bool check && check)
                        {
                            // should discuss how we store Capabilities
                            context.ForwardMessage(null, dec.DecisionActionMessage, null);
                        }
                    }
                    break;
            }

            return Task.FromResult(context);
        }
    }
}
