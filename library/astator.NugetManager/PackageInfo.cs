using Newtonsoft.Json;
using NuGet.Versioning;

namespace astator.NugetManager;

public class PackageInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    [JsonProperty("compile")]
    public List<string> Compile { get; set; } = new();


    public bool Exists(PackageInfo other)
    {
        if (this.Name == other.Name)
        {
            if (this.Version == other.Version)
            {
                return true;
            }
        }
        return false;
    }

    public static bool Exists(List<PackageInfo> left, KeyValuePair<string, NuGetVersion> right)
    {
        foreach (var item in left)
        {
            if (item.Name == right.Key)
            {
                if (item.Version == right.Value.ToString())
                {
                    return true;
                }
            }
        }
        return false;
    }
}