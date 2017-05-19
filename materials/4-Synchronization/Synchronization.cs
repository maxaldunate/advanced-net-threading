/******************************************************************************
Module:  Synchronization.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Wintellect.Threading;

public static class ThreadSynchronization {
   public static void Main() {
      BusyServer(DoWork1);
      BusyServer(DoWork2);
      BusyServer(DoWork3);
   }

   private const Int32 c_numRequests = 30;
   private static CountdownEvent s_cde = new CountdownEvent(c_numRequests);
   private static void BusyServer(WaitCallback doWork) {
      s_cde.Reset(c_numRequests);
      var startTime = Stopwatch.StartNew();
      for (Int32 request = 0; request < c_numRequests; request++) {
         ThreadPool.QueueUserWorkItem(doWork, request);
      }
      s_cde.Wait();
      Console.WriteLine("Time to process={0}", startTime.Elapsed);
   }

   private static readonly Object s_lock = new Object();
   private static void DoWork1(Object request) {
      Monitor.Enter(s_lock);
      Console.WriteLine("Request #{0:00}, Time={1:hh:mm:ss}, Threads={2:00}",
         request, DateTimeOffset.Now, Process.GetCurrentProcess().Threads.Count);

      for (Int64 stop = Environment.TickCount + 1000; Environment.TickCount < stop; ) ;
      Monitor.Exit(s_lock);
      s_cde.Signal();
   }


   private static readonly SemaphoreSlim s_ss = new SemaphoreSlim(1);
   private static async void DoWork2(Object request) {
      await s_ss.WaitAsync();
      Console.WriteLine("Request #{0:00}, Time={1:hh:mm:ss}, Threads={2:00}",
         request, DateTimeOffset.Now, Process.GetCurrentProcess().Threads.Count);

      for (Int64 stop = Environment.TickCount + 1000; Environment.TickCount < stop; ) ;
      s_ss.Release();
      s_cde.Signal();
   }


   private static readonly AsyncReaderWriterLock s_arwl = new AsyncReaderWriterLock();
   private static async void DoWork3(Object request) {
      AccessMode am = (((Int32)request) % 10 == 0) ? AccessMode.Exclusive : AccessMode.Shared;
      await s_arwl.WaitAsync(am);
      Console.WriteLine("Request #{0:00}, Time={1:hh:mm:ss}, Threads={2:00}, Access={3}",
         request, DateTimeOffset.Now, Process.GetCurrentProcess().Threads.Count, am);

      for (Int64 stop = Environment.TickCount + 1000; Environment.TickCount < stop; ) ;
      s_arwl.Release();
      s_cde.Signal();
   }
}

