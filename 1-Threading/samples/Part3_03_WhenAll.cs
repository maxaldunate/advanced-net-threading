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

public static class Part3_WhenAll
{
    public static void Run()
    {
        Task task = Go(100);
        Console.WriteLine("Tasks running");
        Console.ReadLine();
    }

    public static async Task Go(int loops)
    {
        Debugger.Break();
        var requests = new List<Task<string>>(loops);
        for (Int32 n = 0; n < requests.Capacity; n++)
            requests.Add(Part3_NamedPipeClient.IssueClientRequestAsync("http://Wintellect.com", "Request #" + n));

        string[] responses = await Task.WhenAll(requests);

        Debugger.Break();
        for (Int32 n = 0; n < responses.Length; n++)
            Console.WriteLine(responses[n]);
    }

}