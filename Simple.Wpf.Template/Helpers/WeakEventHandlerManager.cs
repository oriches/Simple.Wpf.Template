using System;
using System.Collections.Generic;
using System.Threading;

namespace Simple.Wpf.Template.Helpers;

public static class WeakEventHandlerManager
{
    private static readonly SynchronizationContext SyncContext = SynchronizationContext.Current;

    public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers)
    {
        if (handlers != null)
        {
            var callees = new EventHandler[handlers.Count];
            var count = 0;

            count = CleanupOldHandlers(handlers, callees, count);

            for (var i = 0; i < count; i++) CallHandler(sender, callees[i]);
        }
    }

    private static void CallHandler(object sender, EventHandler eventHandler)
    {
        if (eventHandler != null)
        {
            if (SyncContext != null)
                SyncContext.Post(_ => eventHandler(sender, EventArgs.Empty), null);
            else
                eventHandler(sender, EventArgs.Empty);
        }
    }

    private static int CleanupOldHandlers(IList<WeakReference> handlers, EventHandler[] callees, int count)
    {
        for (var i = handlers.Count - 1; i >= 0; i--)
        {
            var reference = handlers[i];
            if (reference.Target is not EventHandler handler)
            {
                handlers.RemoveAt(i);
            }
            else
            {
                callees[count] = handler;
                count++;
            }
        }

        return count;
    }

    public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler,
        int defaultListSize)
    {
        handlers ??= defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();

        handlers.Add(new WeakReference(handler));
    }

    public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
    {
        if (handlers != null)
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];
                if (reference.Target is not EventHandler existingHandler || existingHandler == handler)
                    handlers.RemoveAt(i);
            }
    }
}