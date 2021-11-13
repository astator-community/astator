using System;

namespace astator.Core.Exceptions
{
    public class AttributeNotExistException : Exception
    {
        public AttributeNotExistException(string key) : base(key + ": 属性不存在!")
        {

        }
    }
}
