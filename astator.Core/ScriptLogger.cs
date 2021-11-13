using Microsoft.Maui;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Time;
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

    public class ScriptTimeSource : TimeSource
    {
        public override DateTime Time => DateTime.Now.AddHours(8);

        public override DateTime FromSystemTime(DateTime systemTime)
        {
            return systemTime;
        }
    }
    public class ScriptLogger
    {
        private static ScriptLogger instance;
        private readonly NLogger logger;
        public List<Action<LogArgs>> Actions = new();

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

        public void AddAction(Action<LogArgs> action)
        {
            this.Actions.Add(action);
        }

        public ScriptLogger()
        {
            TimeSource.Current = new ScriptTimeSource();
            var path = Path.Combine(MauiApplication.Current.FilesDir?.AbsolutePath ?? "/sdcard/astator.log/", "Log", "log.txt");
            var config = new LoggingConfiguration();
            var methodCallTarget = new MethodCallTarget("AddMessage", (logEvent, parameters) =>
            {
                var message = new LogArgs
                {
                    Level = logEvent.Level,
                    Time = DateTime.Now.AddHours(8),
                    Message = logEvent.FormattedMessage
                };

                foreach (var action in this.Actions)
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

            //#if DEBUG
            //            var consoleTarget = new ConsoleTarget
            //            {
            //                Layout = @"[${date:format=HH\:mm\:ss}][${level}]: ${message}",
            //            };
            //            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            //#endif

            this.logger = LogManager.Setup().
                SetupExtensions(s => s.AutoLoadAssemblies(false)).
                LoadConfiguration(config).
                GetCurrentClassLogger();

        }

        public void Log(params object[] items)
        {
            Debug(items);
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
