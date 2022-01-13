using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.NugetManager;

public class DownloadPackageException : Exception
{
    public DownloadPackageException(string pkgId) : base($"下载nuget包: {pkgId}失败!")
    {

    }
}
