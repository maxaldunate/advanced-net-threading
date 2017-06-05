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

public static class Part3_07_AwaitBeginEnd
{
    public static void Run()
    {
        var task = IssueClientRequestAsync("localhost", "Request");
        Console.WriteLine(task.Result);
    }

    public static async Task<string> IssueClientRequestAsync(string serverName, string msg)
    {
        using (var pipe = new NamedPipeClientStream(serverName, "PipeName",
            PipeDirection.InOut, PipeOptions.Asynchronous))
        {
            pipe.Connect(); //before setting read mode
            pipe.ReadMode = PipeTransmissionMode.Message;

            //Asynchronously send data to the server
            Byte[] request = Encoding.UTF8.GetBytes(msg);
            await pipe.WriteAsync(request, 0, request.Length);
            // will return a Task<string> immediatly to function caller

            //Asynchronously read the server's response
            Byte[] response = new Byte[1000];
            Int32 bytesRead = await pipe.ReadAsync(response, 0, response.Length);
            return Encoding.UTF8.GetString(response, 0, bytesRead);
            //** not return to function caller, instead puts in the result of Task
        }
    }
}