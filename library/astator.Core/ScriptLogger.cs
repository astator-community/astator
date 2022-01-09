
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

        private readonly object locker = new();

        public string AddCallback(string key, Action<LogArgs> action)
        {
            lock (locker)
            {
                if (this.callbacks.ContainsKey(key))
                {
                    key += DateTime.Now.ToString("HH-mm-ss-fff");
                }
                this.callbacks.TryAdd(key, action);
                return key;
            }
        }

        public void RemoveCallback(string key)
        {
            lock (locker)
            {
                foreach (var _key in this.callbacks.Keys.ToList())
                {
                    if (_key.StartsWith(key))
                    {
                        this.callbacks.TryRemove(_key, out _);
                    }
                }
            }
        }

        public ScriptLogger()
        {
            var path = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("Log").ToString(), "log.txt");
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
                SetupExtensions(s => s.AutoLoadAssemblies(false)).
                LoadConfiguration(config).
                GetCurrentClassLogger();
        }

        public void Log(params object[] items)
        {
            this.logger.Debug(GetMessage(items));
        }

        public void Debug(params object[] items)
        {
            this.logger.Debug(GetMessage(items));
        }

        public void Info(params object[] items)
        {
            this.logger.Info(GetMessage(items));
        }

        public void Error(params object[] items)
        {
            this.logger.Error(GetMessage(items));
        }

        public void Warn(params object[] items)
        {
            this.logger.Warn(GetMessage(items));
        }

        public void Trace(params object[] items)
        {
            this.logger.Trace(GetMessage(items));
        }

        public void Fatal(params object[] items)
        {
            this.logger.Fatal(GetMessage(items));
        }

        private static StringBuilder GetMessage(params object[] items)
        {
            var message = new StringBuilder();
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0)
                    message.Append(' ');
                message.Append(items[i].ToString());
            }
            return message;
        }
    }
}
