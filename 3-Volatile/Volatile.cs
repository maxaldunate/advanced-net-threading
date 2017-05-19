/******************************************************************************
Module:  Volatile.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/

// Compile with "csc /platform:x86 /o volatile.cs" 
// Use "Start without debugging" to see the behavior

using System;
using System.Threading;

internal static class StrangeBehavior {
   public static void Main() {
      Int32 version = 3;
      switch (version) {
         case 1: Version1.Go(); break;
         case 2: Version2.Go(); break;
         case 3: Version3.Go(); break;
      }
   }

   private static class Version1 {
      private static Boolean s_stopWorker = false;
      public static void Go() {
         Console.WriteLine("Go: Letting worker run for 5 seconds");
         Thread t = new Thread(Worker);
         t.Start();
         Thread.Sleep(5000);
         s_stopWorker = true;
         Console.WriteLine("Go: Waiting for worker to stop");
         t.Join();
         Console.ReadLine();
      }

      private static void Worker(Object o) {
         Int32 x = 0;
         while (!s_stopWorker) x++;
         Console.WriteLine("Worker: Stopped when x={0}", x);
      }
   }

   private static class Version2 {
      private static Boolean s_stopWorker = false;
      public static void Go() {
         Console.WriteLine("Go: Letting worker run for 5 seconds");
         Thread t = new Thread(Worker);
         t.Start();
         Thread.Sleep(5000);
         Volatile.Write(ref s_stopWorker, true);
         Console.WriteLine("Go: Waiting for worker to stop");
         t.Join();
         Console.ReadLine();
      }

      private static void Worker(Object o) {
         Int32 x = 0;
         while (!Volatile.Read(ref s_stopWorker)) x++;
         Console.WriteLine("Worker: Stopped when x={0}", x);
      }
   }

   private static class Version3 {
      private static volatile Boolean s_stopWorker = false;
      public static void Go() {
         Console.WriteLine("Go: Letting worker run for 5 seconds");
         Thread t = new Thread(Worker);
         t.Start();
         Thread.Sleep(5000);
         s_stopWorker = true;
         Console.WriteLine("Go: Waiting for worker to stop");
         t.Join();
         Console.ReadLine();
      }

      private static void Worker(Object o) {
         Int32 x = 0;
         while (!s_stopWorker) x++;
         Console.WriteLine("Worker: Stopped when x={0}", x);
      }
   }
}
