/******************************************************************************
Module:  Threading.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public static class Program {
 
    public static void Main() {

       //Part1_Overhead.Run();
       //Part1_AsyncDelegateExample1.Main();
       //Part1_Responsiveness.Run();

       //Part2_CalcMaxActiveThreads.Run(1, true);
       //Part2_CalcMaxActiveThreads.Run(3, true);
       //Part2_CalcMaxActiveThreads.Run(1, false);

       //Part3_FirstAsyncFunction.Run();

        //Part3_NamedPipeClient.Run();
        //Part3_WhenAll.Run();
        //Part3_WhenAny.Run();

        var x = new Part3_VoidReturnType();
        x.Subscribe();


    }

  
}