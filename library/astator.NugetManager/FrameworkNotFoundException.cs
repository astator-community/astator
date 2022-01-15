namespace astator.NugetManager;

internal class FrameworkNotFoundException : Exception
{
    public FrameworkNotFoundException(string pkgId) : base($"nuget包: {pkgId}不兼容!")
    {

    }
}
