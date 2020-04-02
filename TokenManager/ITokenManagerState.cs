using System.Numerics;

namespace Wetonomy.TokenManager
{
    // We need generic type T because token manager can support Tags
    // Example Tuple(address: string, date: DateTime)
    public interface ITokenManagerState<T>
    {
        public bool Mint(BigInteger amount, T to);
        public bool Burn(BigInteger amount, T from);
        public bool Transfer(BigInteger amount, T from, T to);
    }
}