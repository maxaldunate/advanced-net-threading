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

### Don't block threads **  
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
```csharp
// Two equivalents lines
new Task(Compute, 5).Start();
ThreadPool.QueueUserWorkItem(Compute, 5);

Task<Int32> t = new Task<Int32>(Compute, 5);  //Int32 return type instead of void  
t.Start();  

//Block current thread waiting
t.Wait(); //overloads accept timeout/CancellationToken

t.Result //Get the result (Int32) & calls wait internally

// There are static methods WaitAll & WaitAny returned Task[]
```

















## Part 3: I/O-Bound Async Operations  
[MVA Part 3](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-3-iobound-async-operations-16659?l=DLvEmkitC_4406218965)  


## Part 4: Thread Synchronization Primitives  
[MVA Part 4](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-4-thread-synchronization-primitives-16660?l=1oGCZnitC_8406218965)  


## Part 5: Thread Synchronization Locks
[MVA Part 5](https://mva.microsoft.com/en-US/training-courses/advanced-net-threading-part-5-thread-synchronization-locks-16661?l=A3VXnpitC_9006218965)  


The End