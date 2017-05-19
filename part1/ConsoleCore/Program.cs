using System;
using System.Threading;

namespace ConsoleApp1
{
    public class Program
    {
        private static void ThreadMethod(object state)
        {

        }

        public static void Main(string[] args)
        {
            var t = new Thread(ThreadMethod);
            t.Start("initializationdata");
            t.Join();

        }
    }

}