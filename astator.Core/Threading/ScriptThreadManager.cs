
using System;
using System.Collections.Generic;
using System.Threading;

namespace astator.Core.Threading
{
    public class ScriptThreadManager
    {
        public Action<int> ScriptExitCallback { get; set; }

        public bool ScriptExitSignal { get; set; } = false;

        private readonly List<Thread> threads = new();

        public Thread Start(Action action)
        {
            Thread thread = new(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception)
                {
                    if (!this.ScriptExitSignal)
                    {
                        throw;
                    }
                }
                finally
                {
                    if (this.ScriptExitSignal)
                    {
                        if (IsLastAlive())
                        {
                            this.ScriptExitCallback?.Invoke(1);
                        }
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

        public bool IsAlive()
        {
            foreach (var thread in this.threads)
            {
                if (thread.IsAlive)
                {
                    return true;
                }
            }
            return false;
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
