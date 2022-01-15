namespace astator.NugetManager;

public class DownloadPackageException : Exception
{
    public DownloadPackageException(string pkgId) : base($"下载nuget包: {pkgId}失败!")
    {

    }
}
