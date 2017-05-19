/******************************************************************************
Module:  Threading.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public static class Responsiveness
{
    public static void Run()
    {
        for (Int32 thread = 0; thread < Environment.ProcessorCount; thread++)
            new Thread(InfiniteLoop).Start();
        Thread.Sleep(30000);
        Debugger.Break();
        Console.ReadLine();
    }

    private static void InfiniteLoop(Object o)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Lowest;
        while (true) ;
    }

}