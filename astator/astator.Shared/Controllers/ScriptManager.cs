using Android.Content;
using Android.OS;
using Android.Views;
using astator.Core;
using astator.Core.Engine;
using astator.Core.ThirdParty;
using astator.Core.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application = Android.App.Application;

namespace astator.Controllers
{
    public class ScriptManager
    {
        private static ScriptManager instance;

        private readonly ScriptLogger logger = ScriptLogger.Instance;
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


        public Dictionary<string, ScriptRuntime> Runtimes = new();

        private int step = 0;

        public void GetId(ref string id)
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

        private void RunScript(string path, string id)
        {
            throw new NotImplementedException();
        }

        public async void RunProject(string directory, string id)
        {
            var csprojPath = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)[0];
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);

            GetId(ref id);

            var xd = XDocument.Load(csprojPath);

            var config = xd.Descendants("ScriptConfig");
            var uiMode = Convert.ToBoolean(config.Select(x => x.Element("UIMode")).First()?.Value);
            var entryType = config.Select(x => x.Element("EntryType")).First()?.Value ?? string.Empty;

            if (entryType == string.Empty)
            {
                this.logger.Error("脚本未指定EntryType!");
                return;
            }

            var itemGroup = xd.Descendants("ItemGroup");
            var references = from element in itemGroup.Elements()
                             where element.Name == "Reference"
                             from attr in element.Attributes()
                             where attr.Value.EndsWith(".dll")
                             select attr.Value;


            var engine = new ScriptEngine(Application.Context.GetExternalFilesDir("Sdk").ToString());

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
                engine.LoadScript(script);
            }

            var emitResult = engine.Compile();

            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    this.logger.Error("编译失败: " + item.ToString());
                }
                return;
            }

            this.logger.Log("脚本开始运行: " + id);

            ScriptRuntime runtime;

            if (uiMode)
            {
                var activity = await StartScriptActivity(id);
                runtime = new ScriptRuntime(id, engine, activity, directory);

                Globals.RunOnUiThread(() =>
                {
                    try
                    {
                        engine.Execute(entryType, runtime);
                    }
                    catch (Exception ex)
                    {
                        this.logger.Error(ex.ToString());
                    }
                });
            }
            else
            {
                runtime = new ScriptRuntime(id, engine, directory);

                runtime.Threads.ScriptExitCallback = runtime.Exit;
                runtime.Threads.ScriptExitSignal = true;

                runtime.Tasks.ScriptExitCallback = runtime.Exit;
                runtime.Tasks.ScriptExitSignal = true;

                runtime.Threads.Start(() =>
                  {
                      engine.Execute(entryType, runtime);
                  });
            }
        }

        public void CompileProject(string directory)
        {
            var csprojPath = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)[0];
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);

            this.logger.Log($"开始编译项目: {projectName}");

            var xd = XDocument.Load(csprojPath);

            var config = xd.Descendants("ScriptConfig");
            var entryType = config.Select(x => x.Element("EntryType")).First()?.Value ?? string.Empty;

            if (entryType == string.Empty)
            {
                this.logger.Error("脚本未指定EntryType!");
                return;
            }

            var itemGroup = xd.Descendants("ItemGroup");
            var references = from element in itemGroup.Elements()
                             where element.Name == "Reference"
                             from attr in element.Attributes()
                             where attr.Value.EndsWith(".dll")
                             select attr.Value;


            var engine = new ScriptEngine(Application.Context.GetExternalFilesDir("Sdk").ToString());

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
                engine.LoadScript(script);
            }

            var outputPath = Path.Combine(directory, "output", $"{projectName}.dll");

            var emitResult = engine.Compile(outputPath);

            if (!emitResult.Success)
            {
                foreach (var item in emitResult.Diagnostics)
                {
                    this.logger.Error("编译失败: " + item.ToString());
                }
                return;
            }

            this.logger.Log($"编译成功: {projectName}");

            if (ObfuscatorHelper.Execute(new ObfuscatorRules
            {
                InputPath = outputPath,
                OutputDir = Path.Combine(directory, "output", "obfuscator"),
                EntryType = entryType
            }))
            {
                this.logger.Log($"混淆成功: {projectName}");
            }
        }

        private static async Task<TemplateActivity> StartScriptActivity(string id)
        {
            ContextWrapper context = new ContextThemeWrapper(Application.Context, Android.Resource.Style.Theme);
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
