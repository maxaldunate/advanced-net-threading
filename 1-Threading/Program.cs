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

       Overhead.Run();
       AsyncDelegateExample1.Main();
      
       Responsiveness.Run();

      //CalcMaxActiveThreads(1, true);
      //CalcMaxActiveThreads(3, true);
      //CalcMaxActiveThreads(1, false);

      //FirstAsyncFunction();
   }

   #region Thread Pool Threads
   private const Int32 c_ItemsToProcess = 200;
   private static Int32 s_MaxThreads = 0;
   private static Int32 s_CurrentThreads = 0;
   private static Int32 s_ItemsProcessed = 0;
   private static AutoResetEvent s_are = new AutoResetEvent(false);

   private static void CalcMaxActiveThreads(Int32 affinity, Boolean computeBound) {
      Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)affinity;
      s_MaxThreads = s_CurrentThreads = s_ItemsProcessed = 0;

      Stopwatch stopwatch = Stopwatch.StartNew();
      for (int i = 0; i < c_ItemsToProcess; i++) {
         ThreadPool.QueueUserWorkItem(ActiveWorker, computeBound);
      }
      Console.WriteLine("All items queued");
      s_are.WaitOne();
      Console.WriteLine("{0}: MaxThreads={1}", stopwatch.Elapsed, s_MaxThreads);
      Console.ReadLine();
   }

   private static void ActiveWorker(Object state) {
      // Prolog:
      Int32 crntActive = Interlocked.Increment(ref s_CurrentThreads);
      InterlockedMax(ref s_MaxThreads, crntActive);

      // Method body:
      Boolean computeBound = (Boolean)state;
      if (computeBound) {
         for (Int64 stop = 100 + Environment.TickCount; Environment.TickCount < stop; ) ;
      } else { Thread.Sleep(100); }

      // Epilog:
      Interlocked.Decrement(ref s_CurrentThreads);
      if (Interlocked.Increment(ref s_ItemsProcessed) == c_ItemsToProcess) s_are.Set();
   }

   private static Int32 InterlockedMax(ref Int32 target, Int32 val) {
      Int32 i, j = target;
      do {
         i = j;
         j = Interlocked.CompareExchange(ref target, Math.Max(i, val), i);
      } while (i != j);
      return j;
   }
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