/******************************************************************************
Module:  Threading.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Runtime.CompilerServices;

public static class Part3_07_AwaitBeginEnd
{
    public static void Run()
    {
        var task = StartsServerAsync();
    }

    public static async Task StartsServerAsync()
    {
        while (true)
        {
            var pipe = new NamedPipeServerStream("PipeName");

            await Task.Factory.FromAsync(pipe.BeginWaitForConnection,
                pipe.EndWaitForConnection, null);

            //await ServiceClientRequestAsync(pipe).NoWarning();
        }
    }
}

public static class TaskExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NoWarning(this Task task) { }
}