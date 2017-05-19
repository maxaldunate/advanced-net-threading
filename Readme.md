# Advanced .NET Threading  
MVA  

# Part 1: Thread Funcdamentals

* Threads for robustness, not performance overall  
* One CPU can only run 1 thread at a time
* Perfeormnace in time (cpu) and space (mem allocation)  
* Thread Kernel Object data structure
* Context: x86 = 700 bytes, x64 = 1240, ARM = 350
* TEB: Thread Envorinment Block: 4Kb, exception handling, TSL, GDI/OpenGL
* TLS: Thread local storage
* Stack
   user-mode 1MB
   kernel-mode 12KB/24KB  
* DLL thread attach/detach notifications  



