using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.Core.Exceptions;

internal class SdkNotSupportedException : Exception
{
    public SdkNotSupportedException(int version) : base("当前sdk不支持, 最低版本需要: " + version)
    {

    }
}
