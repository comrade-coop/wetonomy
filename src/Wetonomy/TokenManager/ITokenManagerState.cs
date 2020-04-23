using System.Numerics;

namespace Wetonomy.TokenManager
{
    // We need generic type T because token manager can support Tags
    // Example Tuple(address: string, date: DateTime)
    public interface ITokenManagerState
    {
        public bool Mint(BigInteger amount, IAgentTokenKey to);
        public bool Burn(BigInteger amount, IAgentTokenKey from);
        public bool Transfer(BigInteger amount, IAgentTokenKey from, IAgentTokenKey to);
    }
}