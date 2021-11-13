

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using astator.Core.UI;
using astator.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace astator.Core.Script
{
    public class Manager
    {
        private static Manager instance;
        public static Manager Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new Manager();
                }
                return instance;
            }
        }


        public Dictionary<string, ScriptRuntime> Runtimes = new();

        private int step = 0;
        public void Initialize(string path, string id = null)
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

            if (path.EndsWith(".csproj"))
            {
                RunProject(path, id);
            }
            else if (path.EndsWith(".cs"))
            {
                RunScript(path, id);
            }
        }

        private void RunScript(string path, string id)
        {
            throw new NotImplementedException();
        }

        public async void RunProject(string path, string id)
        {
            var xd = XDocument.Load(path);

            var config = xd.Descendants("ScriptConfig");
            var uiMode = Convert.ToBoolean(config.Select(x => x.Element("UIMode")).First()?.Value);
            var mainType = config.Select(x => x.Element("MainType")).First()?.Value ?? "Main";

            var itemGroup = xd.Descendants("ItemGroup");
            var references = from element in itemGroup.Elements()
                             where element.Name == "Reference"
                             from attr in element.Attributes()
                             where attr.Value.EndsWith(".dll")
                             where !attr.Value.EndsWith("astator.Core.dll")
                             select attr.Value;

            var directory = Path.GetDirectoryName(path);

            var engine = new ScriptEngine();

            foreach (var reference in references)
            {
                if (reference.StartsWith("."))
                {
                    var absolutePath = Path.Combine(directory, reference);
                    if (File.Exists(absolutePath))
                    {
                        engine.LoadFromAssemblyPath(absolutePath);
                    }
                }
                else
                {
                    engine.LoadFromAssemblyPath(reference);
                }

            }

            var scripts = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);

            foreach (var script in scripts)
            {
                engine.LoadScript(script);
            }

            var emitResult = engine.Compile();

            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    ScriptLogger.Instance.Error("编译失败: " + item.ToString());
                }
                return;
            }

            ScriptRuntime runtime;

            if (uiMode)
            {
                var activity = await StartScriptActivity(id);
                runtime = new ScriptRuntime(id, engine, activity, MainActivity.Instance, path);

                activity.OnFinished = () =>
                {
                    runtime.SetExit();
                };
                try
                {
                    engine.Execute(mainType, runtime);
                }
                catch (Exception ex)
                {
                    ScriptLogger.Instance.Error(ex.ToString());
                }
            }
            else
            {
                runtime = new ScriptRuntime(id, engine, MainActivity.Instance, path);

                runtime.Threads.Start(() =>
                {
                    engine.Execute(mainType, runtime);
                });
            }
        }

        private async Task<TemplateActivity> StartScriptActivity(string id)
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

    }
}
