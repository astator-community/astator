using System;

namespace astator.Core.Exceptions;
public class TypeNotSupportedException : Exception
{
    public TypeNotSupportedException(string type) : base(type + ": 类型不支持!")
    {

    }
}
