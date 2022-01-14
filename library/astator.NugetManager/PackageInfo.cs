using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace astator.NugetManager;

public class PackageInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public List<string> Path { get; set; } = new();


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