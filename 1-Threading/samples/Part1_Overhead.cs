/******************************************************************************
Module:  Threading.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public static class Part1_Overhead
{
    public static void Run()
    {
        const Int32 OneMB = 1024 * 1024;
        using (var wakeThreads = new ManualResetEvent(false))
        {
            Int32 threadNum = 0;
            try
            {
                while (true)
                {
                    var t = new Thread(WaitOnEvent);
                    t.Start(wakeThreads);
                    Console.WriteLine("{0}: {1}MB", ++threadNum,
                       Process.GetCurrentProcess().PrivateMemorySize64 / OneMB);
                }
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Out of memory after {0} threads.", threadNum);
                Debugger.Break();
                wakeThreads.Set();   // Release all the threads
            }
        }
    }

    private static void WaitOnEvent(Object eventObj)
    {
        ((ManualResetEvent)eventObj).WaitOne();
        Console.WriteLine("finishing");
    }


}