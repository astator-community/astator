

using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.IO.Compression;

namespace astator.NugetManager;

public class NugetCommands
{
    private const string excludeDlls = @"Microsoft.CSharp
Microsoft.VisualBasic.Core
Microsoft.VisualBasic
Microsoft.Win32.Primitives
Microsoft.Win32.Registry
mscorlib
netstandard
System.AppContext
System.Buffers
System.Collections.Concurrent
System.Collections
System.Collections.Immutable
System.Collections.NonGeneric
System.Collections.Specialized
System.ComponentModel.Annotations
System.ComponentModel.DataAnnotations
System.ComponentModel
System.ComponentModel.EventBasedAsync
System.ComponentModel.Primitives
System.ComponentModel.TypeConverter
System.Configuration
System.Console
System.Core
System.Data.Common
System.Data.DataSetExtensions
System.Data
System.Diagnostics.Contracts
System.Diagnostics.Debug
System.Diagnostics.DiagnosticSource
System.Diagnostics.FileVersionInfo
System.Diagnostics.Process
System.Diagnostics.StackTrace
System.Diagnostics.TextWriterTraceListener
System.Diagnostics.Tools
System.Diagnostics.TraceSource
System.Diagnostics.Tracing
System
System.Drawing
System.Drawing.Primitives
System.Dynamic.Runtime
System.Formats.Asn1
System.Globalization.Calendars
System.Globalization
System.Globalization.Extensions
System.IO.Compression.Brotli
System.IO.Compression
System.IO.Compression.FileSystem
System.IO.Compression.ZipFile
System.IO
System.IO.FileSystem.AccessControl
System.IO.FileSystem
System.IO.FileSystem.DriveInfo
System.IO.FileSystem.Primitives
System.IO.FileSystem.Watcher
System.IO.IsolatedStorage
System.IO.MemoryMappedFiles
System.IO.Pipes.AccessControl
System.IO.Pipes
System.IO.UnmanagedMemoryStream
System.Linq
System.Linq.Expressions
System.Linq.Parallel
System.Linq.Queryable
System.Memory
System.Net
System.Net.Http
System.Net.Http.Json
System.Net.HttpListener
System.Net.Mail
System.Net.NameResolution
System.Net.NetworkInformation
System.Net.Ping
System.Net.Primitives
System.Net.Requests
System.Net.Security
System.Net.ServicePoint
System.Net.Sockets
System.Net.WebClient
System.Net.WebHeaderCollection
System.Net.WebProxy
System.Net.WebSockets.Client
System.Net.WebSockets
System.Numerics
System.Numerics.Vectors
System.ObjectModel
System.Reflection.DispatchProxy
System.Reflection
System.Reflection.Emit
System.Reflection.Emit.ILGeneration
System.Reflection.Emit.Lightweight
System.Reflection.Extensions
System.Reflection.Metadata
System.Reflection.Primitives
System.Reflection.TypeExtensions
System.Resources.Reader
System.Resources.ResourceManager
System.Resources.Writer
System.Runtime.CompilerServices.Unsafe
System.Runtime.CompilerServices.VisualC
System.Runtime
System.Runtime.Extensions
System.Runtime.Handles
System.Runtime.InteropServices
System.Runtime.InteropServices.RuntimeInformation
System.Runtime.Intrinsics
System.Runtime.Loader
System.Runtime.Numerics
System.Runtime.Serialization
System.Runtime.Serialization.Formatters
System.Runtime.Serialization.Json
System.Runtime.Serialization.Primitives
System.Runtime.Serialization.Xml
System.Security.AccessControl
System.Security.Claims
System.Security.Cryptography.Algorithms
System.Security.Cryptography.Cng
System.Security.Cryptography.Csp
System.Security.Cryptography.Encoding
System.Security.Cryptography.OpenSsl
System.Security.Cryptography.Primitives
System.Security.Cryptography.X509Certificates
System.Security
System.Security.Principal
System.Security.Principal.Windows
System.Security.SecureString
System.ServiceModel.Web
System.ServiceProcess
System.Text.Encoding.CodePages
System.Text.Encoding
System.Text.Encoding.Extensions
System.Text.Encodings.Web
System.Text.Json
System.Text.RegularExpressions
System.Threading.Channels
System.Threading
System.Threading.Overlapped
System.Threading.Tasks.Dataflow
System.Threading.Tasks
System.Threading.Tasks.Extensions
System.Threading.Tasks.Parallel
System.Threading.Thread
System.Threading.ThreadPool
System.Threading.Timer
System.Transactions
System.Transactions.Local
System.ValueTuple
System.Web
System.Web.HttpUtility
System.Windows
System.Xml
System.Xml.Linq
System.Xml.ReaderWriter
System.Xml.Serialization
System.Xml.XDocument
System.Xml.XmlDocument
System.Xml.XmlSerializer
System.Xml.XPath
System.Xml.XPath.XDocument
WindowsBase";

    private static string[] ExcludeDlls = excludeDlls.Split("\r\n");

    private static readonly string[] frameworkNames = new string[]
        {
            "net6.0-android31.0",
            "net6.0-android30.0",
            "net6.0",
            "net5.0",
            "netcoreapp3.1",
            "netstandard2.1",
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0",
        };

    public static readonly string NugetDirectory = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("astator").ToString(), "nuget");

    private static readonly ILogger logger = NullLogger.Instance;




    public static async Task<List<IPackageSearchMetadata>> SearchPkgAsync(string text)
    {
        return await Task.Run(async () =>
        {
            var tokensource = new CancellationTokenSource(10000);
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
            var resource = await repository.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrerelease: true);

            var results = await resource.SearchAsync(
            text,
            searchFilter,
            skip: 0,
            take: 30,
            logger,
            tokensource.Token);

            return results.ToList();
        });
    }

    public static async Task<List<IPackageSearchMetadata>> GetPackagesMetadataAsync(string pkgId)
    {
        return await Task.Run(async () =>
        {
            var tokensource = new CancellationTokenSource(10000);
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

            var packages = await resource.GetMetadataAsync(
                pkgId,
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                logger,
                tokensource.Token);

            var result = packages.ToList();
            result.Reverse();
            return result;
        });
    }

    public static async Task<bool> DownLoadPackageAsync(string pkgId, NuGetVersion version)
    {
        return await Task.Run(async () =>
        {
            var tokensource = new CancellationTokenSource(10000);
            var cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            using MemoryStream packageStream = new MemoryStream();

            if (!await resource.CopyNupkgToStreamAsync(
                pkgId,
                version,
                packageStream,
                cache,
                logger,
                tokensource.Token))
            {
                return false;
            }

            var outputDir = Path.Combine(NugetDirectory, pkgId, version.ToString());
            ZipArchive zip = new ZipArchive(packageStream);
            zip.ExtractToDirectory(outputDir, true);

            var libDir = Path.Combine(outputDir, "lib");

            if (Directory.Exists(libDir))
            {
                var framework = string.Empty;
                var frameworks = Directory.GetDirectories(libDir).Select(dir => Path.GetFileName(dir));

                foreach (var name in frameworkNames)
                {
                    if (frameworks.Contains(name))
                    {
                        framework = name;
                        break;
                    }
                }

                foreach (var name in frameworks)
                {
                    if (name != framework)
                    {
                        Directory.Delete(Path.Combine(libDir, name), true);
                    }
                }
            }

            return true;
        });
    }

    public static async Task<List<PackageInfo>> GetPackageInfosAsync(Dictionary<string, NuGetVersion> dependences)
    {
        return await Task.Run(async () =>
        {
            var result = new List<PackageInfo>();
            foreach (var dependence in dependences)
            {
                var dir = Path.Combine(NugetDirectory, dependence.Key, dependence.Value.ToString());

                if (!Directory.Exists(dir))
                {
                    if (!await DownLoadPackageAsync(dependence.Key, dependence.Value))
                    {
                        throw new DownloadPackageException(dependence.Key);
                    }
                }

                var group = await GetPackageDependencyGroupAsync(dependence.Key, dependence.Value);

                if (group is null)
                {
                    continue;
                }

                string path;
                if (group.TargetFramework is null)
                {
                    path = Path.Combine(dir, "lib", $"{dependence.Key}.dll");
                }
                else
                {
                    path = Path.Combine(dir, "lib", group.TargetFramework.GetShortFolderName(), $"{dependence.Key}.dll");
                }

                if (File.Exists(path))
                {
                    result.Add(new PackageInfo
                    {
                        Name = dependence.Key,
                        Version = dependence.Value.ToString(),
                        Path = path,

                    });
                }
            }
            return result;
        });

    }


    public static async Task<Dictionary<string, NuGetVersion>> ListPackageTransitiveDependenceAsync(string pkgId, NuGetVersion version)
    {
        var pkgs = new Dictionary<string, NuGetVersion>
        {
            { pkgId, version }
        };
        return await ListPackageTransitiveDependenceAsync(pkgs);
    }

    public static async Task<Dictionary<string, NuGetVersion>> ListPackageTransitiveDependenceAsync(Dictionary<string, NuGetVersion> pkgs, Dictionary<string, NuGetVersion> parents = null)
    {
        return await Task.Run(async () =>
        {
            if (parents is null)
            {
                parents = new Dictionary<string, NuGetVersion>();
            }

            var dependencyGroup = new Dictionary<string, NuGetVersion>();

            foreach (var pkg in pkgs)
            {
                try
                {
                    var group = await GetPackageDependencyGroupAsync(pkg.Key, pkg.Value);
                    if (group is null)
                    {
                        continue;
                    }

                    foreach (var p in group.Packages)
                    {
                        var version = p.VersionRange.MinVersion;
                        if (dependencyGroup.ContainsKey(p.Id))
                        {
                            if (version > dependencyGroup[p.Id])
                            {
                                dependencyGroup[p.Id] = version;
                            }
                        }
                        else if (pkgs.ContainsKey(p.Id))
                        {
                            if (version > pkgs[p.Id])
                            {
                                pkgs[p.Id] = version;
                            }
                        }
                        else if (parents.ContainsKey(p.Id))
                        {
                            if (version > parents[p.Id])
                            {
                                parents[p.Id] = version;
                            }
                        }
                        else if (!ExcludeDlls.Contains(p.Id))
                        {
                            dependencyGroup.Add(p.Id, version);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if (dependencyGroup.Count > 0)
            {
                var p = parents.Concat(pkgs).ToDictionary(k => k.Key, v => v.Value);
                return await ListPackageTransitiveDependenceAsync(dependencyGroup, p);
            }

            return parents.Concat(pkgs).Concat(dependencyGroup).ToDictionary(k => k.Key, v => v.Value);

        });
    }

    public static async Task<PackageDependencyGroup> GetPackageDependencyGroupAsync(string pkgId, NuGetVersion version)
    {
        return await Task.Run(async () =>
        {
            var path = Path.Combine(NugetDirectory, pkgId, version.ToString(), $"{pkgId}.nuspec");
            if (File.Exists(path))
            {
                var nuspecReader = new NuspecReader(path);
                return GetNearestFrameworkDependencyGroup(nuspecReader.GetDependencyGroups());
            }
            else
            {
                var tokensource = new CancellationTokenSource(10000);
                var cache = new SourceCacheContext();
                var repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
                var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                var info = await resource.GetDependencyInfoAsync(
                     pkgId,
                     version,
                     cache,
                     logger,
                     tokensource.Token);

                return GetNearestFrameworkDependencyGroup(info.DependencyGroups);
            }

        });
    }

    public static async Task<NuGetVersion> ParseFloatingVersion(string pkgId, string version)
    {
        if (!version.EndsWith("*"))
        {
            return NuGetVersion.Parse(version);
        }

        var tokensource = new CancellationTokenSource(10000);
        var cache = new SourceCacheContext();
        var repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
        var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

        var versions = await resource.GetAllVersionsAsync(pkgId, cache, logger, tokensource.Token);

        return versions.FindBestMatch(VersionRange.Parse(version), version => version);
    }

    public static PackageDependencyGroup GetNearestFrameworkDependencyGroup(IEnumerable<PackageDependencyGroup> DependencyGroups)
    {
        var groups = new Dictionary<string, PackageDependencyGroup>();

        foreach (var group in DependencyGroups)
        {
            var framework = group.TargetFramework;
            var name = framework.GetShortFolderName();
            groups.Add(name, group);
        }

        foreach (var name in frameworkNames)
        {
            if (groups.ContainsKey(name))
            {
                return groups[name];
            }
        }

        return null;
    }
}
