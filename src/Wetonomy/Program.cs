using System;

namespace Wetonomy
{
    public abstract class TokenPair<T>
    {
        public abstract string GetAgentId();

        public abstract T GetTag();
    }

    public class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
