using Android.Content;
using Android.OS;
using Android.Views;
using astator.Core;
using astator.Core.Engine;
using astator.Core.Script;
using astator.Core.UI;
using System.Collections.Concurrent;
using System.Reflection;
using Application = Android.App.Application;

namespace astator.Controllers;

public class ScriptManager
{
    private static ScriptManager instance;
    public static ScriptManager Instance
    {
        get
        {
            if (instance is null)
            {
                instance = new ScriptManager();
            }
            return instance;
        }
    }

    private readonly ConcurrentDictionary<string, ScriptRuntime> runtimes = new();

    private int step = 0;

    private readonly object locker = new();

    public void GetId(ref string id)
    {
        lock (this.locker)
        {
            if (id is null)
            {
                id = $"script-{this.step}";
            }
            else
            {
                id = $"{id}-{this.step}";
            }

            this.step++;
        }
    }

    public async Task<ScriptRuntime> RunScript(string path)
    {
        return await Task.Run(async () =>
         {
             var id = Path.GetFileNameWithoutExtension(path);
             GetId(ref id);

             var projectDir = Path.GetDirectoryName(path);

             var engine = new ScriptEngine(projectDir);

             if (!await engine.Restore())
             {
                 engine.RemoveTipsFloaty();
                 return null;
             }

             engine.ParseAllCS();

             var emitResult = engine.Compile();
             if (!emitResult.Success)
             {
                 foreach (var item in emitResult.Diagnostics)
                 {
                     Logger.Error("编译失败: " + item.ToString());
                 }
                 return null;
             }

             var method = engine.GetScriptEntryMethodInfo(Path.GetFileName(path));
             if (method is null)
             {
                 Logger.Error("未找到入口方法!");
                 return null;
             }
             var isUiMode = method.GetCustomAttribute<ScriptEntryMethod>().IsUIMode;

             ScriptLogger.Log("脚本开始运行: " + id);

             ScriptRuntime runtime;

             if (isUiMode)
             {
                 var activity = await StartScriptActivity(id);
                 runtime = new ScriptRuntime(id, engine, activity, projectDir);

                 Device.BeginInvokeOnMainThread(() =>
                 {
                     try
                     {
                         ScriptEngine.Execute(method, runtime);
                     }
                     catch (Exception ex)
                     {
                         Logger.Error(ex.ToString());
                     }
                 });
             }
             else
             {
                 runtime = new ScriptRuntime(id, engine, projectDir);

                 runtime.Threads.Start(() =>
                 {
                     ScriptEngine.Execute(method, runtime);
                 });
             }

             this.runtimes.TryAdd(id, runtime);

             return runtime;
         });
    }

    public async Task<ScriptRuntime> RunProject(string projectDir)
    {
        return await Task.Run(async () =>
        {
            var csprojPath = Directory.GetFiles(projectDir, "*.csproj", SearchOption.AllDirectories).First();

            var id = Path.GetFileNameWithoutExtension(csprojPath);
            GetId(ref id);

            var engine = new ScriptEngine(projectDir);

            if (!await engine.Restore())
            {
                return null;
            }

            engine.ParseAllCS();

            var emitResult = engine.Compile();
            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    Logger.Error("编译失败: " + item.ToString());
                }
                return null;
            }

            var method = engine.GetProjectEntryMethodInfo();
            if (method is null)
            {
                Logger.Error("未找到入口方法!");
                return null;
            }
            var isUiMode = method.GetCustomAttribute<ProjectEntryMethod>().IsUIMode;

            ScriptLogger.Log("脚本开始运行: " + id);

            ScriptRuntime runtime;

            if (isUiMode)
            {
                var activity = await StartScriptActivity(id);
                runtime = new ScriptRuntime(id, engine, activity, projectDir);

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        ScriptEngine.Execute(method, runtime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                });
            }
            else
            {
                runtime = new ScriptRuntime(id, engine, projectDir);

                runtime.Threads.Start(() =>
                {
                    ScriptEngine.Execute(method, runtime);
                });
            }

            this.runtimes.TryAdd(id, runtime);

            return runtime;
        });
    }

    private static async Task<TemplateActivity> StartScriptActivity(string id)
    {
        ContextWrapper context = new ContextThemeWrapper(Application.Context, Resource.Style.AppTheme_NoActionBar);
        var intent = new Intent(context, typeof(TemplateActivity)).AddFlags(ActivityFlags.NewTask);

        var bundle = new Bundle();
        bundle.PutString("id", id);

        intent.PutExtras(bundle);
        context.StartActivity(intent);

        return await Task.Run(() =>
        {
            while (true)
            {
                if (TemplateActivity.ScriptActivityList.ContainsKey(id))
                {
                    return TemplateActivity.ScriptActivityList[id];
                }
                Thread.Sleep(50);
            }
        });
    }

    public void Stop(string key)
    {
        foreach (var _key in this.runtimes.Keys.ToList())
        {
            if (_key.Equals(key))
            {
                this.runtimes.TryRemove(key, out var runtime);
                runtime.SetStop();
            }

        }
    }

    public void StopAll()
    {
        foreach (var key in this.runtimes.Keys.ToList())
        {
            this.runtimes.TryRemove(key, out var runtime);
            runtime.SetStop();
        }
    }
}
