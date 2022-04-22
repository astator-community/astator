using ALogger = astator.LoggerProvider.AstatorLogger;

namespace astator.Core.Script;

public static class Logger
{
    public static void Log(string msg) => ALogger.Trace(msg);
    public static void Trace(string msg) => ALogger.Trace(msg);
    public static void Debug(string msg) => ALogger.Debug(msg);
    public static void Info(string msg) => ALogger.Info(msg);
    public static void Error(string msg) => ALogger.Error(msg);
    public static void Warn(string msg) => ALogger.Warn(msg);
    public static void Fatal(string msg) => ALogger.Fatal(msg);
    public static void Log(params object[] items) => ALogger.Log(items);
    public static void Trace(params object[] items) => ALogger.Trace(items);
    public static void Debug(params object[] items) => ALogger.Debug(items);
    public static void Info(params object[] items) => ALogger.Info(items);
    public static void Error(params object[] items) => ALogger.Error(items);
    public static void Warn(params object[] items) => ALogger.Warn(items);
    public static void Fatal(params object[] items) => ALogger.Fatal(items);
}
