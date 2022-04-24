using astator.LoggerProvider;
using System.Text;

namespace astator.Modules.Base;

public class ConsoleOut : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(bool value) => AstatorLogger.Log(value);
    public override void Write(char value) => AstatorLogger.Log(value);
    public override void Write(char[] buffer) => AstatorLogger.Log(new string(buffer));
    public override void Write(char[] buffer, int index, int count) => AstatorLogger.Log(new string(buffer, index, count));
    public override void Write(decimal value) => AstatorLogger.Log(value);
    public override void Write(double value) => AstatorLogger.Log(value);
    public override void Write(int value) => AstatorLogger.Log(value);
    public override void Write(long value) => AstatorLogger.Log(value);
    public override void Write(object value) => AstatorLogger.Log(value);
    public override void Write(float value) => AstatorLogger.Log(value);
    public override void Write(string value) => AstatorLogger.Log(value);
    public override void Write(string format, object arg0) => AstatorLogger.Log(string.Format(format, arg0));
    public override void Write(string format, object arg0, object arg1) => AstatorLogger.Log(string.Format(format, arg0, arg1));
    public override void Write(string format, object arg0, object arg1, object arg2)
        => AstatorLogger.Log(string.Format(format, arg0, arg1, arg2));
    public override void Write(string format, params object[] arg) => AstatorLogger.Log(string.Format(format, arg));
    public override void Write(uint value) => AstatorLogger.Log(value);
    public override void Write(ulong value) => AstatorLogger.Log(value);

    public override void WriteLine(bool value) => AstatorLogger.Log(value);
    public override void WriteLine(char value) => AstatorLogger.Log(value);
    public override void WriteLine(char[] buffer) => AstatorLogger.Log(new string(buffer));
    public override void WriteLine(char[] buffer, int index, int count) => AstatorLogger.Log(new string(buffer, index, count));
    public override void WriteLine(decimal value) => AstatorLogger.Log(value);
    public override void WriteLine(double value) => AstatorLogger.Log(value);
    public override void WriteLine(int value) => AstatorLogger.Log(value);
    public override void WriteLine(long value) => AstatorLogger.Log(value);
    public override void WriteLine(object value) => AstatorLogger.Log(value);
    public override void WriteLine(float value) => AstatorLogger.Log(value);
    public override void WriteLine(string value) => AstatorLogger.Log(value);
    public override void WriteLine(string format, object arg0) => AstatorLogger.Log(string.Format(format, arg0));
    public override void WriteLine(string format, object arg0, object arg1) => AstatorLogger.Log(string.Format(format, arg0, arg1));
    public override void WriteLine(string format, object arg0, object arg1, object arg2)
        => AstatorLogger.Log(string.Format(format, arg0, arg1, arg2));
    public override void WriteLine(string format, params object[] arg) => AstatorLogger.Log(string.Format(format, arg));
    public override void WriteLine(uint value) => AstatorLogger.Log(value);
    public override void WriteLine(ulong value) => AstatorLogger.Log(value);

}
