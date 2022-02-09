
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using NLogger = NLog.Logger;

namespace astator.Core
{
    public class LogArgs
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
        public DateTime Time { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    public class ScriptLogger
    {
        private static ScriptLogger instance;

        public static ScriptLogger Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new ScriptLogger();
                }
                return instance;
            }
        }

        private readonly NLogger logger;

        private readonly ConcurrentDictionary<string, Action<LogArgs>> callbacks = new();


        private static readonly object locker = new();

        public static string AddCallback(string key, Action<LogArgs> action)
        {
            lock (locker)
            {
                if (Instance.callbacks.ContainsKey(key))
                {
                    key += DateTime.Now.ToString("HH-mm-ss-fff");
                }
                Instance.callbacks.TryAdd(key, action);
                return key;
            }
        }

        public static void RemoveCallback(string key)
        {
            lock (locker)
            {
                foreach (var _key in Instance.callbacks.Keys.ToList())
                {
                    if (_key.StartsWith(key))
                    {
                        Instance.callbacks.TryRemove(_key, out _);
                    }
                }
            }
        }

        public ScriptLogger()
        {
            var path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("log").ToString(), "log.txt");
            var config = new LoggingConfiguration();
            var methodCallTarget = new MethodCallTarget("AddMessage", (logEvent, parameters) =>
            {
                var message = new LogArgs
                {
                    Level = logEvent.Level,
                    Time = DateTime.Now,
                    Message = logEvent.FormattedMessage
                };

                foreach (var action in this.callbacks.Values)
                {
                    try
                    {
                        action.Invoke(message);
                    }
                    catch { }
                }
            });
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, methodCallTarget));
            var fileTarget = new FileTarget
            {
                FileName = path,
                Layout = @"${level}*/${date::universalTime=false:format=MM-dd HH\:mm\:ss\.fff}*/: ${message}",
            };
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));

            this.logger = LogManager.Setup().
                SetupExtensions(s => s.AutoLoadAssemblies(true)).
                LoadConfiguration(config).
                GetCurrentClassLogger();
        }

        public static void Log(params object[] items)
        {
            Instance.logger.Debug(GetMessage(items));
        }

        public static void Debug(params object[] items)
        {
            Instance.logger.Debug(GetMessage(items));
        }

        public static void Info(params object[] items)
        {
            Instance.logger.Info(GetMessage(items));
        }

        public static void Error(params object[] items)
        {
            Instance.logger.Error(GetMessage(items));
        }

        public static void Warn(params object[] items)
        {
            Instance.logger.Warn(GetMessage(items));
        }

        public static void Trace(params object[] items)
        {
            Instance.logger.Trace(GetMessage(items));
        }

        public static void Fatal(params object[] items)
        {
            Instance.logger.Fatal(GetMessage(items));
        }

        private static StringBuilder GetMessage(params object[] items)
        {
            var message = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                {
                    message.Append(' ');
                }

                message.Append(items[i].ToString());
            }
            return message;
        }
    }
}
