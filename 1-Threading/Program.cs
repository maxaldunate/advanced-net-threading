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

       Part1_Overhead.Run();
       Part1_AsyncDelegateExample1.Main();
       Part1_Responsiveness.Run();

      Part2_CalcMaxActiveThreads.Run(1, true);
      //CalcMaxActiveThreads(3, true);
      //CalcMaxActiveThreads(1, false);

      //FirstAsyncFunction();
   }

   #region Thread Pool Threads





   #endregion

   #region Async/Await
   private static void FirstAsyncFunction() {
      Int32 id1 = Environment.CurrentManagedThreadId;
      Task<Int32> task = HttpLengthAsync("http://Wintellect.com/");

      Debugger.Break();
      Int32 id3 = Environment.CurrentManagedThreadId; // Same as id1
      Console.ReadLine();
      Debugger.Break();
      var length = task.Result;  // Waits for Task complete to get its result
   }

   private static async Task<Int32> HttpLengthAsync(String uri) {
      Int32 id2 = Environment.CurrentManagedThreadId; // Same as FirstAsyncFunction's id1
      Task<String> task = new HttpClient().GetStringAsync(uri);

      // await lets calling thread return to caller (Main)
      // Code after await resumes via thread pool thread
      String text = await task;
      Debugger.Break();
      Int32 id3 = Environment.CurrentManagedThreadId; // A thread pool thread
      return text.Length;  // Sets Task's Result property
   }
   #endregion
}


//CalcMaxActiveThreads((IntPtr)7, true);
//CalcMaxActiveThreads((IntPtr)((1 << Environment.ProcessorCount) - 1), true);