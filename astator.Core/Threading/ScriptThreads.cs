
using System;
using System.Collections.Generic;
using System.Threading;

namespace astator.Core.Threading
{
    public class ScriptThreads
    {
        private readonly List<Thread> threads = new();

        private readonly Action exitCallback;

        public ScriptThreads(Action callback)
        {
            this.exitCallback = callback;
        }
        public Thread Start(Action action)
        {
            Thread thread = new(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (ThreadInterruptedException) { }
                catch (Exception ex)
                {
                    ScriptLogger.Instance.Error(ex);
                }
                finally
                {
                    if (IsLastAlive())
                    {
                        this.exitCallback?.Invoke();
                    }
                }
            });
            thread.Start();
            this.threads.Add(thread);
            return thread;
        }

        private bool IsLastAlive()
        {
            var num = 0;
            foreach (var thread in this.threads)
            {
                if (thread.IsAlive)
                {
                    num++;
                }
            }
            return num <= 1;
        }

        public void Interrupt()
        {
            foreach (var thread in this.threads)
            {
                thread.Interrupt();
            }
        }
    }
}
