using System.IO;
using System.Text;

namespace astator.Core
{
    public class ScriptConsole : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        //private readonly TextWriter defaultWriter;

        private readonly ScriptLogger logger;
        public ScriptConsole()
        {
            this.logger = ScriptLogger.Instance;
            //defaultWriter = Console.Out;
            //Console.SetOut(this);
        }

        //protected new void Dispose()
        //{
        //    Console.SetOut(defaultWriter);
        //}

        //public override void Write(bool value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(object value)
        //{
        //    if (value is not null)
        //    {
        //        logger.Log(value);
        //    }
        //}
        //public override void Write(char value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(char[] value)
        //{
        //    if (value is not null)
        //    {
        //        logger.Log(value);
        //    }
        //}

        //public override void Write(decimal value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(double value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(float value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(int value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(long value)
        //{
        //    logger.Log(value);
        //}

        //public override void Write(char[] buffer, int index, int count)
        //{
        //    base.Write(buffer, index, count);
        //}

        //public override void Write(ReadOnlySpan<char> buffer)
        //{
        //    base.Write(buffer);
        //}

        //public override void Write(string format, object arg0)
        //{
        //    base.Write(format, arg0);
        //}

        //public override void Write(string format, object arg0, object arg1)
        //{
        //    base.Write(format, arg0, arg1);
        //}
        //public override void Write(string format, object arg0, object arg1, object arg2)
        //{
        //    base.Write(format, arg0, arg1, arg2);
        //}

        //public override void Write(string format, params object[] arg)
        //{
        //    base.Write(format, arg);
        //}

        //public override void Write(string value)
        //{
        //    base.Write(value);
        //}

        //public override void Write(StringBuilder value)
        //{
        //    base.Write(value);
        //}

        //public override void Write(uint value)
        //{
        //    base.Write(value);
        //}

        //public override void Write(ulong value)
        //{
        //    base.Write(value);
        //}

        //public override void WriteLine()
        //{
        //    base.WriteLine();
        //}

        //public override void WriteLine(bool value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(char value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(char[] buffer)
        //{
        //    base.WriteLine(buffer);
        //}

        //public override void WriteLine(decimal value)
        //{
        //    base.WriteLine(value);
        //}


        //public override void WriteLine(double value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(float value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(int value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(long value)
        //{
        //    base.WriteLine(value);
        //}


        //public override void WriteLine(object value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(ReadOnlySpan<char> buffer)
        //{
        //    base.WriteLine(buffer);
        //}

        //public override void WriteLine(string format, object arg0)
        //{
        //    base.WriteLine(format, arg0);
        //}

        //public override void WriteLine(string format, object arg0, object arg1)
        //{
        //    base.WriteLine(format, arg0, arg1);
        //}

        //public override void WriteLine(string format, object arg0, object arg1, object arg2)
        //{
        //    base.WriteLine(format, arg0, arg1, arg2);
        //}

        //public override void WriteLine(string format, params object[] arg)
        //{
        //    base.WriteLine(format, arg);
        //}

        public override void WriteLine(string value)
        {
            this.logger.Log(value);
        }

        //public override void WriteLine(StringBuilder value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(uint value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(ulong value)
        //{
        //    base.WriteLine(value);
        //}

        //public override void WriteLine(char[] buffer, int index, int count)
        //{
        //    base.WriteLine(buffer, index, count);
        //}
    }
}
