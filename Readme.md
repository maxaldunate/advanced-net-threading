# Advanced .NET Threading  
[MVA](https://mva.microsoft.com/en-us/training-courses/advanced--net-threading-part-1-thread-fundamentals-16656)  
Copyright (c) by Jeffrey Richter  
from Wintellect

## Part 1: Thread Fundamentals

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
















