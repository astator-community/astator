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

        private readonly ScriptLogger logger;
        public ScriptConsole()
        {
            this.logger = ScriptLogger.Instance;
        }

        public override void Write(bool value)
        {
            this.logger.Log(value);
        }

        public override void Write(object value)
        {
            this.logger.Log(value);
        }
        public override void Write(char value)
        {
            this.logger.Log(value);
        }

        public override void Write(char[] buffer)
        {
            var value = new string(buffer);
            this.logger.Log(value);
        }

        public override void Write(decimal value)
        {
            this.logger.Log(value);
        }

        public override void Write(double value)
        {
            this.logger.Log(value);
        }

        public override void Write(float value)
        {
            this.logger.Log(value);
        }

        public override void Write(int value)
        {
            this.logger.Log(value);
        }

        public override void Write(long value)
        {
            this.logger.Log(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            this.logger.Log(value);
        }

        public override void Write(ReadOnlySpan<char> buffer)
        {
            var value = new string(buffer);
            this.logger.Log(value);
        }

        public override void Write(string format, object arg0)
        {
            var value = string.Format(format, arg0);
            this.logger.Log(value);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            var value = string.Format(format, arg0, arg1);
            this.logger.Log(value);
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            var value = string.Format(format, arg0, arg1, arg2);
            this.logger.Log(value);
        }

        public override void Write(string format, params object[] arg)
        {
            var value = string.Format(format, arg);
            this.logger.Log(value);
        }

        public override void Write(string value)
        {
            this.logger.Log(value);
        }

        public override void Write(StringBuilder value)
        {
            this.logger.Log(value);
        }

        public override void Write(uint value)
        {
            this.logger.Log(value);
        }

        public override void Write(ulong value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine()
        {
            this.logger.Log();
        }

        public override void WriteLine(bool value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(decimal value)
        {
            this.logger.Log(value);
        }


        public override void WriteLine(double value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(float value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(int value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(long value)
        {
            this.logger.Log(value);
        }


        public override void WriteLine(object value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(char value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(char[] buffer)
        {
            var value = new string(buffer);
            this.logger.Log(value);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            var value = new string(buffer, index, count);
            this.logger.Log(value);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            var value = new string(buffer);
            this.logger.Log(value);
        }

        public override void WriteLine(string format, object arg0)
        {

            var value = string.Format(format, arg0);
            this.logger.Log(value);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {

            var value = string.Format(format, arg0, arg1);
            this.logger.Log(value);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {

            var value = string.Format(format, arg0, arg1, arg2);
            this.logger.Log(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            var value = string.Format(format, arg);
            this.logger.Log(value);
        }

        public override void WriteLine(string value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(StringBuilder value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(uint value)
        {
            this.logger.Log(value);
        }

        public override void WriteLine(ulong value)
        {
            this.logger.Log(value);
        }


        public override Task WriteAsync(char value)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            });
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer, index, count);
                logger.Log(value);
            });
        }

        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer.ToArray());
                logger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteAsync(string value)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            });
        }

        public override Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteLineAsync(char value)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            });
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer, index, count);
                logger.Log(value);
            });
        }

        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var value = new string(buffer.ToArray());
                logger.Log(value);
            }, cancellationToken);
        }

        public override Task WriteLineAsync(string value)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            });
        }

        public override Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                logger.Log(value);
            }, cancellationToken);
        }
    }
}
