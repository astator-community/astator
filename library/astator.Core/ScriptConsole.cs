using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Core
{
    public class ScriptConsole : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public ScriptConsole()
        {
        }

        public override void Write(bool value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(object value)
        {
            ScriptLogger.Log(value);
        }
        public override void Write(char value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(char[] buffer)
        {
            var value = new string(buffer);
            ScriptLogger.Log(value);
        }

        public override void Write(decimal value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(double value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(float value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(int value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(long value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            ScriptLogger.Log(value);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            var value = new string(buffer);
            ScriptLogger.Log(value);
        }

        public override void Write(string format, object arg0)
        {
            var value = string.Format(format, arg0);
            ScriptLogger.Log(value);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            var value = string.Format(format, arg0, arg1);
            ScriptLogger.Log(value);
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            var value = string.Format(format, arg0, arg1, arg2);
            ScriptLogger.Log(value);
        }

        public override void Write(string format, params object[] arg)
        {
            var value = string.Format(format, arg);
            ScriptLogger.Log(value);
        }

        public override void Write(string value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(StringBuilder value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(uint value)
        {
            ScriptLogger.Log(value);
        }

        public override void Write(ulong value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine()
        {
            ScriptLogger.Log();
        }

        public override void WriteLine(bool value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(decimal value)
        {
            ScriptLogger.Log(value);
        }


        public override void WriteLine(double value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(float value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(int value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(long value)
        {
            ScriptLogger.Log(value);
        }


        public override void WriteLine(object value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(char value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(char[] buffer)
        {
            var value = new string(buffer);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            ScriptLogger.Log(value);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            var value = new string(buffer);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(string format, object arg0)
        {

            var value = string.Format(format, arg0);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {

            var value = string.Format(format, arg0, arg1);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {

            var value = string.Format(format, arg0, arg1, arg2);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            var value = string.Format(format, arg);
            ScriptLogger.Log(value);
        }

        public override void WriteLine(string value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(StringBuilder value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(uint value)
        {
            ScriptLogger.Log(value);
        }

        public override void WriteLine(ulong value)
        {
            ScriptLogger.Log(value);
        }


        public override Task WriteAsync(char value)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer, index, count);
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer.ToArray());
                ScriptLogger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteAsync(string value)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteLineAsync(char value)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer, index, count);
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer.ToArray());
                ScriptLogger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteLineAsync(string value)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            });
        }

        public override Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                ScriptLogger.Log(value);
            }, cancellationToken);
        }
    }
}
