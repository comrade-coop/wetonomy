using System;

namespace Wetonomy
{
    interface ITokenPair<T>
    {
        T GetTag();
        T GetAgentId();
    }

    class TokenPair : ITokenPair<string>
    {
        public string GetAgentId()
        {
            throw new NotImplementedException();
        }

        public string GetTag()
        {
            throw new NotImplementedException();
        }
    }
    public class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
