
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly NLogger logger;
        public Dictionary<string, Action<LogArgs>> Callbacks = new();

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

        public string AddCallback(string key, Action<LogArgs> action)
        {
            while (this.Callbacks.ContainsKey(key))
            {
                key += DateTime.Now.ToString("dd-HH-mm-ss");
            }

            this.Callbacks.Add(key, action);
            return key;
        }

        public void RemoveCallback(string key)
        {
            foreach (var _key in this.Callbacks.Keys)
            {
                if (_key.StartsWith(key))
                {
                    this.Callbacks.Remove(_key);
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

                foreach (var action in this.Callbacks.Values)
                {
                    action.Invoke(message);
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
