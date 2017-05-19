/******************************************************************************
Module:  AwaitableEvent.cs
Notices: Copyright (c) by Jeffrey Richter and Wintellect
******************************************************************************/

using System;
using System.Threading.Tasks;

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

#if false
   public sealed class EventDispatcher<TEventArgs> {
      private readonly EventInfo m_eventInfo;
      public EventDispatcher(Type type, String eventName) {
         m_eventInfo = type.GetTypeInfo().DeclaredEvents.Where(ei => ei.Name == eventName).First();
      }
      public Disposer Add(Object target, Action<Object, TEventArgs> handler) {
         m_eventInfo.AddEventHandler(target, handler);
         return new Disposer(() => { Remove(target, handler); });
      }
      public void Remove(Object target, Action<Object, TEventArgs> handler) {
         m_eventInfo.RemoveEventHandler(target, handler);
      }

      public struct Disposer : IDisposable {
         private readonly Action m_delegate;
         public Disposer(Action @delegate) { m_delegate = @delegate; }
         public void Dispose() { m_delegate(); }
      }
#if false
#region No Reflection
   private Action<Object, TEventArgs> m_handlers = null;
   public Disposer Add(Action<TEventArgs> handler) {
      m_handlers = (Action<Object, TEventArgs>)Delegate.Combine(m_handlers, handler);
      return new Disposer(() => Remove(handler));
   }
   public void Remove(Action<TEventArgs> handler) {
      m_handlers = (Action<Object, TEventArgs>)Delegate.Remove(handler, m_handlers);
   }
   public void Handler(Object sender, TEventArgs args) {
      if (m_handlers == null) return;
      m_handlers(sender, args);
   }
#endregion
#endif
   }
#endif
