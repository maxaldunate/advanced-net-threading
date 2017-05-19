using System;
using System.Threading;

namespace ThreadingPart1
{
    public static class Program
    {
        private static void ThreadMethod(object state){

        }

        public static void Main(){
            var t = new Thread(ThreadMethod);
            t.Start("initializationdata");
            t.Join();

        }
    }
    
}