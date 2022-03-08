using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Core.Threading
{
    //在astator中, 脚本必须经过此api来实现task, 否则astator无法在脚本停止时卸载相关程序集

    /// <summary>
    /// 脚本task管理类
    /// </summary>
    public class TaskManager
    {
        internal Action<int> ScriptExitCallback { get; set; }
        internal bool ScriptExitSignal { get; set; } = false;

        private readonly List<Task> tasks = new();

        private readonly ConcurrentDictionary<int, CancellationTokenSource> tokenSources = new();

        private void CallBackInvoke()
        {
            if (this.ScriptExitSignal)
            {
                if (IsLastAlive())
                {
                    this.ScriptExitCallback?.Invoke(2);
                }
            }
        }

        /// <summary>
        /// 向所有task发送取消信号
        /// </summary>
        public void Cancel()
        {
            foreach (var source in this.tokenSources.Values)
            {
                source.Cancel();
            }
        }

        /// <summary>
        /// 是否有task存活
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            foreach (var task in this.tasks)
            {
                if (task.Status == TaskStatus.Running
                    || task.Status == TaskStatus.WaitingForActivation
                    || task.Status == TaskStatus.WaitingForChildrenToComplete
                    || task.Status == TaskStatus.WaitingToRun)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否只有一个task存活
        /// </summary>
        /// <returns></returns>
        private bool IsLastAlive()
        {
            var num = 0;
            foreach (var task in this.tasks)
            {
                if (task.Status == TaskStatus.Running
                    || task.Status == TaskStatus.WaitingForActivation
                    || task.Status == TaskStatus.WaitingForChildrenToComplete
                    || task.Status == TaskStatus.WaitingToRun)
                {
                    num++;
                }
            }
            return num <= 1;
        }

        /// <summary>
        /// 获取task对应的CancellationTokenSource
        /// </summary>
        /// <param name="id">task的id</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public CancellationTokenSource GetTokenSource(int id)
        {
            if (this.tokenSources.ContainsKey(id))
            {
                return this.tokenSources[id];
            }
            throw new KeyNotFoundException(id.ToString());
        }

        /// <summary>
        /// 以任务方式执行一个action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task Run(Action<CancellationToken> action)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    action.Invoke(source.Token);
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
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task Run(Action<CancellationToken> action, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(() =>
            {
                try
                {
                    action.Invoke(source.Token);
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
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task Run(Func<CancellationToken, Task> action)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                try
                {
                    await action.Invoke(source.Token);
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
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task Run(Func<CancellationToken, Task> action, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(async () =>
            {
                try
                {
                    await action.Invoke(source.Token);
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
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<CancellationToken, TResult> func)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    return func(source.Token);
                }
                catch (Exception)
                {
                    if (this.ScriptExitSignal)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<CancellationToken, TResult> func, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(() =>
            {
                try
                {
                    return func(source.Token);
                }
                catch (Exception)
                {
                    if (this.ScriptExitSignal)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }


        public Task<TResult> Run<TResult>(Func<CancellationToken, Task<TResult>> func)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                try
                {
                    return await func(source.Token);
                }
                catch (Exception)
                {
                    if (this.ScriptExitSignal)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<CancellationToken, Task<TResult>> func, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(async () =>
            {
                try
                {
                    return await func(source.Token);
                }
                catch (TaskCanceledException)
                {
                    if (this.ScriptExitSignal)
                    {
                        return default;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    CallBackInvoke();
                }
            }, source.Token);

            this.tasks.Add(task);
            this.tokenSources.TryAdd(task.Id, source);

            return task;
        }
    }
}
