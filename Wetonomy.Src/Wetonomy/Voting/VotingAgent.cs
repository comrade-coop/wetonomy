using System;
using System.Collections;
using System.Collections.Generic;
using Apocryph.Agents.Testbed.Api;
using Wetonomy.Voting.Messages;
using Wetonomy.Voting.Publications;
using System.Linq;

namespace Wetonomy.Voting
{
    public static class VotingAgent<T,V> where V : IEnumerable
    {
        public class VotingState
        {
            //Just Temporary solution
            public int nonce = 0;

            public IVoteStategy<T,V> VotingStategy;
            public Dictionary<string, Decision<T>> Decisions = new Dictionary<string, Decision<T>>();
            public Dictionary<string, Dictionary<string, V>> DecisionsVotes = new Dictionary<string, Dictionary<string, V>>();

            public Dictionary<string, AgentCapability> Capabilities = new Dictionary<string, AgentCapability>();
        }

        public static AgentContext<VotingState> Run(object state, AgentCapability self, object message)
        {
            var context = new AgentContext<VotingState>(state as VotingState, self);

            switch (message)
            {
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
                            context.SendMessage(null, dec.DecisionActionMessage, null);
                        }
                    }

                    break;
            }

            return context;
        }
    }
}
