
using System;
using System.Collections.Generic;
using System.Threading;

namespace astator.Core.Threading
{
    //在astator中, 脚本必须经过此api来实现线程创建, 否则astator无法在脚本停止时卸载相关程序集

    /// <summary>
    /// 脚本线程管理类
    /// </summary>
    public class ScriptThreadManager
    {
        internal Action<int> ScriptExitCallback { get; set; }

        internal bool ScriptExitSignal { get; set; } = false;

        private readonly List<Thread> threads = new();

        /// <summary>
        /// 以线程方式执行一个action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 是否只有一个线程存活
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 是否有线程存活
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 向所有线程发送中断信号
        /// </summary>
        public void Interrupt()
        {
            foreach (var thread in this.threads)
            {
                thread.Interrupt();
            }
        }
    }
}
