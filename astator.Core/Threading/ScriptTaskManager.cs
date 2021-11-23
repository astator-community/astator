using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Core.Threading
{
    public class ScriptTaskManager
    {
        public Action<int> ScriptExitCallback { get; set; }
        public bool ScriptExitSignal { get; set; } = false;

        private readonly List<Task> tasks = new();
        private readonly List<CancellationTokenSource> tokenSources = new();

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

        public void Cancel()
        {
            foreach (var source in this.tokenSources)
            {
                source.Cancel();
            }
        }

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

        public Task Run(Action action)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    action();
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
            this.tokenSources.Add(source);

            return task;
        }
        public Task Run(Action action, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(() =>
            {
                try
                {
                    action();
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
            this.tokenSources.Add(source);

            return task;
        }

        public Task Run(Func<Task> action)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                try
                {
                    await action();
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
            this.tokenSources.Add(source);

            return task;
        }

        public Task Run(Func<Task> action, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(async () =>
            {
                try
                {
                    await action();
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
            this.tokenSources.Add(source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<TResult> func)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                try
                {
                    return func();
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
            this.tokenSources.Add(source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<TResult> func, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(() =>
            {
                try
                {
                    return func();
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
            this.tokenSources.Add(source);

            return task;
        }


        public Task<TResult> Run<TResult>(Func<Task<TResult>> func)
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(async () =>
            {
                try
                {
                    return await func();
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
            this.tokenSources.Add(source);

            return task;
        }

        public Task<TResult> Run<TResult>(Func<Task<TResult>> func, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            var task = Task.Run(async () =>
            {
                try
                {
                    return await func();
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
            this.tokenSources.Add(source);

            return task;
        }
    }
}
