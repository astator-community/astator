using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.NugetManager;

internal class FrameworkNotFoundException:Exception
{
    public FrameworkNotFoundException(string pkgId) : base($"nuget包: {pkgId}不兼容!")
    {

    }
}
