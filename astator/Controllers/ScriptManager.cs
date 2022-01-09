using Android.Content;
using Android.OS;
using Android.Views;
using astator.Core;
using astator.Core.Engine;
using astator.Core.Script;
using astator.Core.UI;
using System.Collections.Concurrent;
using System.Reflection;
using System.Xml.Linq;
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

    private static ScriptLogger Logger => ScriptLogger.Instance;

    private readonly ConcurrentDictionary<string, ScriptRuntime> runtimes = new();

    private int step = 0;

    private readonly object locker = new();

    public void GetId(ref string id)
    {
        lock (locker)
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

    public async void RunScript(string path, string id)
    {
        await Task.Run(async () =>
        {
            GetId(ref id);

            var directory = Path.GetDirectoryName(path);
            var engine = new ScriptEngine();

            engine.ParseCsx(path);

            var emitResult = engine.Compile();

            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    Logger.Error("编译失败: " + item.ToString());
                }
                return;
            }

            var method = engine.GetEntryMethod();
            if (method is null)
            {
                Logger.Error("未找到入口方法!");
                return;
            }
            var isUiMode = method.GetCustomAttribute<EntryMethod>().IsUIMode;

            Logger.Log("脚本开始运行: " + id);

            ScriptRuntime runtime;

            if (isUiMode)
            {
                var activity = await StartScriptActivity(id);
                runtime = new ScriptRuntime(id, engine, activity, directory);

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        engine.Execute(method, runtime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                });
            }
            else
            {
                runtime = new ScriptRuntime(id, engine, directory);

                runtime.Threads.Start(() =>
                {
                    engine.Execute(method, runtime);
                });
            }

            this.runtimes.TryAdd(id, runtime);
        });
    }

    public async void RunProject(string directory, string id)
    {
        await Task.Run(async () =>
        {
            var csprojPath = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)[0];

            GetId(ref id);

            var xd = XDocument.Load(csprojPath);

            var config = xd.Descendants("ScriptConfig");


            var itemGroup = xd.Descendants("ItemGroup");
            var references = from element in itemGroup.Elements()
                             where element.Name == "Reference"
                             from attr in element.Attributes()
                             where attr.Value.EndsWith(".dll")
                             select attr.Value;


            var engine = new ScriptEngine();

            foreach (var reference in references)
            {
                if (reference.StartsWith("."))
                {
                    var absolutePath = Path.Combine(directory, reference);
                    if (File.Exists(absolutePath))
                    {
                        engine.LoadReference(absolutePath);
                    }
                }
                else
                {
                    engine.LoadReference(reference);
                }
            }

            var scripts = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

            foreach (var script in scripts)
            {
                engine.ParseScriptFromFile(script);
            }

            var emitResult = engine.Compile();

            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    Logger.Error("编译失败: " + item.ToString());
                }
                return;
            }

            var method = engine.GetEntryMethod();
            if (method is null)
            {
                Logger.Error("未找到入口方法!");
                return;
            }
            var isUiMode = method.GetCustomAttribute<EntryMethod>().IsUIMode;

            Logger.Log("脚本开始运行: " + id);

            ScriptRuntime runtime;

            if (isUiMode)
            {
                var activity = await StartScriptActivity(id);
                runtime = new ScriptRuntime(id, engine, activity, directory);

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        engine.Execute(method, runtime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                });
            }
            else
            {
                runtime = new ScriptRuntime(id, engine, directory);

                runtime.Threads.Start(() =>
                {
                    engine.Execute(method, runtime);
                });
            }

            this.runtimes.TryAdd(id, runtime);
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
        foreach (var _key in runtimes.Keys.ToList())
        {
            if (_key.Equals(key))
            {
                runtimes.TryRemove(key, out var runtime);
                runtime.SetStop();
            }

        }
    }

    public void StopAll()
    {
        foreach (var key in runtimes.Keys.ToList())
        {
            runtimes.TryRemove(key, out var runtime);
            runtime.SetStop();
        }
    }
}
