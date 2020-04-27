using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Wetonomy.TokenActionAgents.State;
using Wetonomy.TokenManager;
using Wetonomy.TokenManager.Messages;

namespace Wetonomy.TokenActionAgents.Strategies.Split
{
	public class WeightedSplitStrategy: ITriggeredAction
    {
        public (IList<object>, IList<object>) Execute(RecipientState state, AbstractTrigger message)
        {
            var messagesResult = new List<object>();
            BigInteger amount = message.Amount;
            int count = state.Recipients.Count;
            if (count == 0) throw new Exception();
            double weightsSum = 0;
            foreach (TokenPairKey<double> recipient in state.Recipients)
            {
                weightsSum += recipient.GetTag();
            }


            BigInteger portion = amount / (int)weightsSum;
            
            // We are going to lose tokens because we are using integer
            foreach (TokenPairKey<double> recipient in state.Recipients)
            {
                //msg.To.ChangeAgentId(state.SelfId)
                var command = new TransferTokenMessage(portion, new SingleAngentTokenKey(state.SelfId), recipient);
                amount -= portion;
                messagesResult.Add(command);
            }
            return (messagesResult, null);
        }
    }
}
