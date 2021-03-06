# Advanced .NET Threading  
Copyright (c) by Jeffrey Richter  
from Wintellect

## Part 1: Thread Fundamentals
[MVA Part 1](https://mva.microsoft.com/en-us/training-courses/advanced--net-threading-part-1-thread-fundamentals-16656)  

### Introduction

* Threads for robustness, not performance overall  
* One CPU can only run 1 thread at a time (after quantum)
* Perfeormnace in time (cpu) and space (mem allocation)  
* Thread Kernel Object data structure
* Context: x86 = 700 bytes, x64 = 1240, ARM = 350
* TEB: Thread Envorinment Block: 4Kb, exception handling, TSL, GDI/OpenGL
* TLS: Thread local storage
* Stack  
   user-mode 1MB  
   kernel-mode 12KB/24KB  
* DLL thread attach/detach notifications  
* Performance impact not just switch contect more for invalidate all cpu cache each time
* Looks like all at same time but quantum time... 30 milliseconds
* Most thread are waiting... so waste space but not time, are waiting for  
   keyboard, mouse, network, file inputs, etc.  
* Task Manager  
   Cores & Logical processors  
   Processes & Threads  
   Details add threads column
   Memmory: working set, peak, delta, shared & private 

### Demo. Thread Overhead

* 1510 thread per process... bad question
* run without start
* many time spent on start
* F10 debbuger feature: suspend all threads
* Garbage collector: suspend all threads

### Thread Scheculing

* Reasons to use threading ... cpu time left
* Visual studio compiling during type
* File indexing when search in windows 
* Consider battery usage & heat 
* Spell checking & defragment disk
* Thread Sleep example vs Timer at the thread pool

### Threads Priorities

* Values: Highest, AboveNormal, Normal, BelowNormal, Lowest
* Lowest for long running tasks threads
* Avoid raising a thread's priority

### WinRT Threading APIs & Windows Store Apps

* WinRT APIs not have dedicated threads, no sleep & no change priority
* WinRT have Windows.System.thread with ThreadPool & ThreadPoolTimer
* Background Win Store apps have all thread suspended (batt & foregrounf app improve)


## Part 2: Compute-Bound Async Operations  
[MVA Part 2](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-2-computebound-async-operations-16658?l=fG7K1fitC_2206218965)  

### Don't block threads
* You tempted to create more threads  
* Garbagge collector have to suspend all threads  
* Degrade debugging performance  
* Avoid blocking threads ... by performing asynchronous operations  
	* Compute bound  
	* I/O bound

### The CLR's thread pool  

If work items queue quickly, more threads are created  
If work items reduce, TP threads sit idle  

Affinity ... bit mask to use specific cores  
Stopwatch ... accurately measure elapsed time  
Interlocked ... Increment/Decrement/CompareExchange  
AutoResetEvent ... notifies a waiting thread that an event has occurred  
ProcessorAffinity ...  doesen't work
[ProcessorAffinity](https://stackoverflow.com/questions/2510593/how-can-i-set-processor-affinity-in-net)  
** Putting a thread to sleep vs making threads to work throu TickCount **  
If the sleep time increase ... you will have a greater number of threads  

### The standard cooperative cancellation pattern  
CancellationTokenSource & ThreadPool.QueueUserWorkItem

### System.Threading.Tasks or Tasks Parallel Library  

### Getting a Task's Result  

```csharp
// Two equivalents lines
new Task(Compute, 5).Start();
ThreadPool.QueueUserWorkItem(Compute, 5);

Task<Int32> t = new Task<Int32>(Compute, 5);  //Int32 return type instead of void  
t.Start();  // Starts the task sometime later (puts the task in the queue)

//Block current thread waiting
t.Wait(); //overloads accept timeout/CancellationToken

t.Result //Get the result (Int32) & calls wait internally

// There are static methods WaitAll & WaitAny returned Task[]
```

###  Automatically Starting a New Task when another Task Completes

```cs
var t = new Task<Int32>(Compute, 5);
t.Start();
//Ok
t.ContinueWith(t => Console.WriteLine(t.Result),
		TaskContinuationOptions.OnlyOnRanToCompletion);
//Exception
t.ContinueWith(t => Console.WriteLine(t.Exception),
		TaskContinuationOptions.OnlyOnFaulted);
//Cancellation
t.ContinueWith(t => Console.WriteLine("Canceled"),
		TaskContinuationOptions.OnlyOnCanceled);
```

###  Cancelling a Task

Tasks can return a value, Int32 in the example. So cancellations throws an exception.  
```cs
private static Int32 Compute(Object state) {
	var ct = (CancellationToken) state;
	var result = 0;
	for (var n = 0; n < 10000; n++) {
		ct.ThrowIfCacellationRequested();
		// Do work here ...
	}
	return result;
}
```

###  A task may start a child tasks
```cs
var parent = new Task<Int32[]>( () => {
	var results = new Int32[2];
	new Task(() => results[0] = Sum(10000)).Start();
	new Task(() => results[1] = Sum(20000)).Start();
	return results;
});
parent.Start();

//Problem: this runs when parent finishes (but not its children)
parent.ContinueWith(t => Array.ForEach(t.Result, r => m_lb.Items.Add(r)));
```

Solution with no threads blocking
AttachedToParent flag increments the counter at the parent task
```cs
	new Task(() => results[0] = Sum(10000),
		TaskCreationOptions.AttachedToParent).Start();
```

GUI thread can manipulate m_lb UI element
Run at the other thread
```cs
var ts = TaskScheduler.FromCurrentSynchronizationContext();
parent.ContinueWith(t => Array.ForEach(t.Result, r => m_lb.Items.Add(r)), ts);
```

###  Tasks and dependencies

Dependecy tree
* A
	* B
		* D
	* C
		* E
		* F
```cs
var solution = new Task(() => {
	var tf = new TaskFactory(
		TaskcreationOptions.AttachedToParent,
		TaskContinuationOptions.AttachedToParent);

	var D = tf.StartNew(() => Compile("D"));
	var E = tf.StartNew(() => Compile("E"));
	var F = tf.StartNew(() => Compile("F"));

	var B = D.ContinueWith(t => Compile("B"));
	var C = tf.ContinueWhenAll(new Task[]{E, F}, task => Compile("C"));
	var A = tf.ContinueWhenAll(new Task[]{B, C}, task => Compile("A"));
});

solution.Start();
```

###  System.Threading.Parallel's Static  
For/ForEach/Invoke Methods  
* Internally uses Tasks class 
* Independent operation
* you don't know the order
* no return untill operations complete  
```cs
//Static For
for (Int32 i=0; i<1000; i++) DoWork(i);
Parallel.For(0, 1000, i=>DoWork(i));

//Static ForEach
for (var item in collection) DoWork(item);
Parallel.ForEach(collection, item => DoWork(item));

// Invoke parallel works
DoWork1(); DoWork2(); DoWork3();
Parallel.Invoke(() => DoWork1(), () => DoWork2(), () => DoWork3());
```

###  Periodically Performing an Asynchronous Compute-Bound Operation

* Verions 1 to udse a timer   
� Que pasa si "Status" vuelve a ser llamado, 2 segundos despues, pero a�n no ha terminado ?  
```cs
internal static class TimerDemo {
	private static Timer s_timer;
	public static void Main(){
		Console.WriteLine("Checking status every 2 seconds:");
		s_timer = new Timer(Status, null, 0, 2000);
		Console.ReadLine();
	}
	private static void Status(Object state){
		// Chack status code here ....
	}
}

```
Solution or version 2  
Possible s_timer.Change called before initialization
```cs
internal static class TimerDemo {
	private static Timer s_timer;
	public static void Main(){
		Console.WriteLine("Checking status every 2 seconds:");
		s_timer = new Timer(Status, null, 0, Timeout.infinite); //No 2000
		Console.ReadLine();
	}
	private static void Status(Object state){
		// Chack status code here ....

		// Have the Time call this method again in 2 seconds
		s_timer.Change(2000, Timeout.infinite);
	}
}

```

Complete solution, version 3: The Good Version

```cs
internal static class TimerDemo {
	private static Timer s_timer;
	public static void Main(){
		Console.WriteLine("Checking status every 2 seconds:");
		s_timer = new Timer(Status, null, Timeout.infinite, Timeout.infinite); //Change
		s_timer.Change(0, Timeout.infinite); //Change
		Console.ReadLine();
	}
	private static void Status(Object state){
		// Chack status code here ....

		// Have the Time call this method again in 2 seconds
		s_timer.Change(2000, Timeout.infinite);
	}
}
```

Try not to use thread synchronization to solve race conditions  


## Part 3: I/O-Bound Async Operations  
[MVA Part 3](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-3-iobound-async-operations-16659?l=DLvEmkitC_4406218965)  

### Synchronous I/O
In C#, process to read a file with FileStream:  
* Our manage code
* Native code
* Win32 function ReadFile
	* Allocate I/O Request Packet data structure (IRP)
* Thread jumps from user mode to kernel mode
	* Passes the IRP down into the kernel
* Passes to hard disk queue
* Thread blocks while hardware does I/O
* When completes all the way up the thread stack

### Asynchronous I/O with XxxAsync
* FileStream (..., FileOptions.Asynchronous)
	* Don't block thread requesting I/O
	* Put completed IRP in ThreadPool
	* `Async methods` return a task I/O operation will complete in the future
	* `await` keyword

### Method using Async/Await
See demo code at �Part3_FirstAsyncFunction�
```cs
//async create an state ma chine with stop & resume states
private static async Task<Int32> HttpLengthAsync(string uri){
	var text = await new HttpClient().GetStringAsync(uri);
	//At await machine stops & resume with the result
	return text.Length
	//return put the value in the Result methos of the task
}

//Equivalent
private static async Task<Int32> HttpLengthAsync(string uri){
	Task<string> task = new HttpClient().GetStringAsync(uri);
	vat text = await task;
	return text.Length;
}
```

### Compiler Trasnformation from await to an state machine  
* new structure : `IAsyncStateMachine`
* `AsyncTaskMethodBuilder<Int32>`
* `TaskAwaiter<string>`

### Async Function Limitations
* No async on following mehtods
	* Main... will terminate the process
	* constructors... return really to garbage collector or CLR creating the object
	* props get/set... cos not allow to mark the get & set like Task<> return type
	* event add/remove... similar as before
* No out or ref parameters
* No await operator in catch, finally or unsafe block
* No await operator in lock w/thread ownership
	* No C# lock or Monitor Enter/Exit
	* Use SemaphoreSlim.WaitAsync instead
* In a query expression, await only allowed within
	* First collection expression of the initial from clause
	* Collection expression of join clause

### Named Pipe Client
```cs
    public static async Task<string> IssueClientRequestAsync(string serverName, string msg)
    {

        using (var pipe = new NamedPipeClientStream(serverName, "PipeName",
            PipeDirection.InOut, PipeOptions.Asynchronous))
        {

            pipe.Connect(); //before setting read mode
            pipe.ReadMode = PipeTransmissionMode.Message;

            //Asynchronously sen data to the server
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
```

### Some Async Functions in the FCL (Fw class library)
* Stream derived types
* TextReader derived types
* TextWriter derived types
* HttpClient new in 4.5 replace of HttpWebRequest class
* SqlCommand
* Tools (WSDL.exe & SvcUtil.exe) producing WS proxies

### Non-Scalable Servers
Browser-ASP.NET Server-SQL Server  
Blocking threads till max thread support for a single process (1.530)  
Context switching at response  
**Conclussion**  
This Architecture the server really spends most os its time creating threads, 
context switching, destroying threads, which is really the worst possible scenario.  
 
### Scalable Servers
With Async calls to sql server

### Implementing Asynchronous Servers
* ASP.NET Web Form Page
* ASP.NET MVC Controller: derive from AsyncController & return Task<ActionResult>
* ASP.NET HTTP Handler: derive from HttpTaskAsyncHandler & override ProcessRequestAsync
* WCF Service: implemt as a async function returning Tas or Task<TResult>

### Task.WhenAll
Multiple I/O request and continue when all finished  
```cs
    public static async Task Go()
    {
        var requests = new List<Task<string>>(10000);
        for (Int32 n = 0; n < requests.Capacity; n++)
            requests.Add(IssueClientRequestAsync("localhost", "Request #" + n));

        string[] responses = await Task.WhenAll(requests);

        for (Int32 n = 0; n < responses.Length; n++)
            Console.WriteLine(responses[n]);
    }
```
### Task.WhenAny
Processing resualts at each one are complete  
```cs
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
```
### Async Function Return Types & WinRT Deferrals
Possible void return for async, not just Task or Task<TResult>  
Just for the event handlers scenarios  
Strongly discouraged  
```cs
	async void OnSuspending(object sender, SuspendingEventArgs e) {
        // A dewferral tells Windows you're returning but not done
        var deferral = e.Suspendingoperation.GetDeferral();
        // TODO: perform async operation(s) here ...
        var result = await xxxAsync();
        deferral.Complete();  //Now, tell Windows we're done
	}
```
[Deferrals](https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.background.backgroundtaskdeferral)  
Telling windows I'm returning but I'm not done  

### Async Lambda Expressions
Example at Part3_06_AsyncLambdaExpressions.cs  

### Await Begin/end Methods
Example at Part3_07_AwaitBeginEnd.cs

### Asynchronous Event Handlers. Tic-Tac-Toe Game Loop Logic
* Initializre
* Wait for PointerPressed event
* if empty..
* if not 3 ina  row ...
* switch X->O or O->X 
* if 3 in a row ... winner dialog & re-Initialize  
   
* The game loop is a state machine  
  **Like async functions!!!**  

**Interesting example to implement asynchronous state machines**  
```cs
namespace Wintellect.AwaitableEvent {
   public sealed class AwaitableEvent<TEventArgs> {
      private TaskCompletionSource<AwaitableEventArgs<TEventArgs>> m_tcs;

      // Returns an (awaitable) Task; set when EventHandler is invoked
      public Task<AwaitableEventArgs<TEventArgs>> RaisedAsync(Object state = null) {
         if (m_tcs == null)
            m_tcs = new TaskCompletionSource<AwaitableEventArgs<TEventArgs>>(state);
         return m_tcs.Task;
      }

      // Invoked when event is raised
      public void Handler(Object sender, TEventArgs eventArgs) {
         if (m_tcs == null) return;
         // We use the temporary variable (tcs) & reset m_tcs to null before calling
         // SetResult because SetResult returns from await immediately which may
         // call RaisedAsync again and we need this to create a new TaskCompletionSource
         var tcs = m_tcs; m_tcs = null;
         tcs.SetResult(new AwaitableEventArgs<TEventArgs>(sender, eventArgs));
      }
   }

   public sealed class AwaitableEventArgs<TEventArgs> {
      public readonly Object Sender;
      public readonly TEventArgs Args;
      internal AwaitableEventArgs(Object sender, TEventArgs args) {
         Sender = sender;
         Args = args;
      }
   }
}
```

Interesting for ussagge...  
`for (Boolean winner = false; !winner; ) {}`

### Applications Models & their Threading models  

* Apps impose their own threading model
  * CUI (Console User Interface) & NT Services no model (free threaded)
  * GUI: windows must be modified by thread that creates it
  * ASP.NET: WebForm, MVC, web services: impersonates client's culture/identity
    [Set the Culture and UI Culture for ASP.NET Web Page Globalization](http://msdn.microsoft.com/en-us/library/bz9tc508.aspx)
* SynchronizationCOntext-derived objects connect and application model to its threading model
* The await operator captures the calling SC object and ensures that any continuation occurs via this SC object
  * For application code, this is usually good
  * For class library code, this is usually bad
    Bad if you try to use that library in different kind of apps


### GUI App deadlocks

Solved with `ConfigureAwait(false);` not calling SynchronizationContext object and improve performance

Another way: Task.Run Forces use of Thread Pool Threads

### Not using .NET 4.5 yet?

* Pre-release Async Targeting Pack (VS 2012)  
  * Nuget pkg Microsoft.Bcl.Async
  * Supports fwk 4.0
* Jeffrey Richter's Power Threading library contains `AsyncEnumerator` (VS 2005)

## Part 4: Thread Synchronization Primitives  
[MVA Part 4](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-4-thread-synchronization-primitives-16660?l=1oGCZnitC_8406218965)  

### Thread Synchronization Mindset

* at the same time
* error prone: no force to acquire permission
* bad perf: requesting permission is slow & other threads stop running
* increase overhead: blocking threads cause creation of new threads
```cs
private sealed class LinkedList{
	private var m_lock = new SomeKindOfLock();
	private Node m_head;

	public void Add(Node node){
		m_lock.Acquire();
		newNode.m_next = m_head;
		m_head = newNode;
		m_lock.Release();
	}
}
```
**How to avoid**  
* Prefer unshared objects
  * heap objects
  * reead-only objects (immutables like string)
  * delegates
  * builders 
  * value types

  ** It's only if the thread that creates the object passes a reference to other thread**

### Class Libraries & Thread Safety

* FCL (Framework Class Library) guarantees static methods are thread safe
* FCL doesn't guarantee instance methods are thread safe
  * If the porpouse of the object is to sinchronize thread, is thread safe
   * CancellationTokenSource class
   * Class wrapping any kind of locks like Monitor class enter and leave methods
   * Reader-Writer lock slim acquire & release methods
* Your own class libraries should mimic these rules

### CLR's Memory Model: Volatile Methods













## Part 5: Thread Synchronization Locks
[MVA Part 5](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-5-thread-synchronization-locks-16661?l=A3VXnpitC_9006218965)  


The End