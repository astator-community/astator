namespace astator.NugetManager;

public class InstallPackageException : Exception
{
    public InstallPackageException(string pkgId) : base($"下载nuget包: {pkgId}失败!")
    {

    }
}
