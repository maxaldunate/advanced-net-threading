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

public static class Part3_WhenAny
{
    public static void Run()
    {
        Task task = Go(100);
        Console.WriteLine("Tasks running");
        Console.ReadLine();
    }

    public static async Task Go(int loops)
    {
        var requests = new List<Task<string>>(loops);
        for (Int32 n = 0; n < requests.Capacity; n++)
            requests.Add(Part3_NamedPipeClient
                .IssueClientRequestAsync("localhost", "Request #" + n));

        // Continue as EACH task completes
        while (requests.Count > 0)
        {
            var responseTask = await Task.WhenAny(requests);
            requests.Remove(responseTask);
            Console.WriteLine(responseTask.Result);
        }
    }

}