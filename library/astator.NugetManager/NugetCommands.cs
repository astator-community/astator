

using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.IO.Compression;

namespace astator.NugetManager;

public class NugetCommands
{

    private static readonly string[] excludeDlls = new[]
    {
        //net6.0
        "Microsoft.CSharp",
        "Microsoft.Win32.Primitives",
        "mscorlib",
        "netstandard",
        "System.AppContext",
        "System.Buffers",
        "System.Collections.Concurrent",
        "System.Collections",
        "System.Collections.Immutable",
        "System.Collections.NonGeneric",
        "System.Collections.Specialized",
        "System.ComponentModel.Annotations",
        "System.ComponentModel.DataAnnotations",
        "System.ComponentModel",
        "System.ComponentModel.EventBasedAsync",
        "System.ComponentModel.Primitives",
        "System.ComponentModel.TypeConverter",
        "System.Configuration",
        "System.Console",
        "System.Core",
        "System.Data.Common",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Diagnostics.Contracts",
        "System.Diagnostics.Debug",
        "System.Diagnostics.DiagnosticSource",
        "System.Diagnostics.FileVersionInfo",
        "System.Diagnostics.Process",
        "System.Diagnostics.StackTrace",
        "System.Diagnostics.TextWriterTraceListener",
        "System.Diagnostics.Tools",
        "System.Diagnostics.TraceSource",
        "System.Diagnostics.Tracing",
        "System",
        "System.Drawing",
        "System.Drawing.Primitives",
        "System.Dynamic.Runtime",
        "System.Formats.Asn1",
        "System.Globalization.Calendars",
        "System.Globalization",
        "System.Globalization.Extensions",
        "System.IO.Compression.Brotli",
        "System.IO.Compression",
        "System.IO.Compression.FileSystem",
        "System.IO.Compression.ZipFile",
        "System.IO",
        "System.IO.FileSystem.AccessControl",
        "System.IO.FileSystem",
        "System.IO.FileSystem.DriveInfo",
        "System.IO.FileSystem.Primitives",
        "System.IO.FileSystem.Watcher",
        "System.IO.IsolatedStorage",
        "System.IO.MemoryMappedFiles",
        "System.IO.Pipes.AccessControl",
        "System.IO.Pipes",
        "System.IO.UnmanagedMemoryStream",
        "System.Linq",
        "System.Linq.Expressions",
        "System.Linq.Parallel",
        "System.Linq.Queryable",
        "System.Memory",
        "System.Net",
        "System.Net.Http",
        "System.Net.Http.Json",
        "System.Net.HttpListener",
        "System.Net.Mail",
        "System.Net.NameResolution",
        "System.Net.NetworkInformation",
        "System.Net.Ping",
        "System.Net.Primitives",
        "System.Net.Requests",
        "System.Net.Security",
        "System.Net.ServicePoint",
        "System.Net.Sockets",
        "System.Net.WebClient",
        "System.Net.WebHeaderCollection",
        "System.Net.WebProxy",
        "System.Net.WebSockets.Client",
        "System.Net.WebSockets",
        "System.Numerics",
        "System.Numerics.Vectors",
        "System.ObjectModel",
        "System.Reflection.DispatchProxy",
        "System.Reflection",
        "System.Reflection.Emit",
        "System.Reflection.Emit.ILGeneration",
        "System.Reflection.Emit.Lightweight",
        "System.Reflection.Extensions",
        "System.Reflection.Metadata",
        "System.Reflection.Primitives",
        "System.Reflection.TypeExtensions",
        "System.Resources.Reader",
        "System.Resources.ResourceManager",
        "System.Resources.Writer",
        "System.Runtime.CompilerServices.Unsafe",
        "System.Runtime.CompilerServices.VisualC",
        "System.Runtime",
        "System.Runtime.Extensions",
        "System.Runtime.Handles",
        "System.Runtime.InteropServices",
        "System.Runtime.InteropServices.RuntimeInformation",
        "System.Runtime.Intrinsics",
        "System.Runtime.Loader",
        "System.Runtime.Numerics",
        "System.Runtime.Serialization",
        "System.Runtime.Serialization.Formatters",
        "System.Runtime.Serialization.Json",
        "System.Runtime.Serialization.Primitives",
        "System.Runtime.Serialization.Xml",
        "System.Security.AccessControl",
        "System.Security.Claims",
        "System.Security.Cryptography.Algorithms",
        "System.Security.Cryptography.Cng",
        "System.Security.Cryptography.Csp",
        "System.Security.Cryptography.Encoding",
        "System.Security.Cryptography.OpenSsl",
        "System.Security.Cryptography.Primitives",
        "System.Security.Cryptography.X509Certificates",
        "System.Security",
        "System.Security.Principal",
        "System.Security.Principal.Windows",
        "System.Security.SecureString",
        "System.ServiceModel.Web",
        "System.ServiceProcess",
        "System.Text.Encoding.CodePages",
        "System.Text.Encoding",
        "System.Text.Encoding.Extensions",
        "System.Text.Encodings.Web",
        "System.Text.Json",
        "System.Text.RegularExpressions",
        "System.Threading.Channels",
        "System.Threading",
        "System.Threading.Overlapped",
        "System.Threading.Tasks.Dataflow",
        "System.Threading.Tasks",
        "System.Threading.Tasks.Extensions",
        "System.Threading.Tasks.Parallel",
        "System.Threading.Thread",
        "System.Threading.ThreadPool",
        "System.Threading.Timer",
        "System.Transactions",
        "System.Transactions.Local",
        "System.ValueTuple",
        "System.Web",
        "System.Web.HttpUtility",
        "System.Windows",
        "System.Xml",
        "System.Xml.Linq",
        "System.Xml.ReaderWriter",
        "System.Xml.Serialization",
        "System.Xml.XDocument",
        "System.Xml.XmlDocument",
        "System.Xml.XmlSerializer",
        "System.Xml.XPath",
        "System.Xml.XPath.XDocument",
        "WindowsBase",

        //maui
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.Configuration.Abstractions",
        "Microsoft.Extensions.Configuration.Binder",
        "Microsoft.Extensions.Configuration.CommandLine",
        "Microsoft.Extensions.Configuration.EnvironmentVariables",
        "Microsoft.Extensions.Configuration.FileExtensions",
        "Microsoft.Extensions.Configuration.Json",
        "Microsoft.Extensions.Configuration.UserSecrets",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Abstractions",
        "Microsoft.Extensions.FileProviders.Abstractions",
        "Microsoft.Extensions.FileProviders.Physical",
        "Microsoft.Extensions.FileSystemGlobbing",
        "Microsoft.Extensions.Hosting",
        "Microsoft.Extensions.Hosting.Abstractions",
        "Microsoft.Extensions.Logging",
        "Microsoft.Extensions.Logging.Abstractions",
        "Microsoft.Extensions.Logging.Configuration",
        "Microsoft.Extensions.Logging.Console",
        "Microsoft.Extensions.Logging.Debug",
        "Microsoft.Extensions.Logging.EventLog",
        "Microsoft.Extensions.Logging.EventSource",
        "Microsoft.Extensions.Options",
        "Microsoft.Extensions.Options.ConfigurationExtensions",
        "Microsoft.Extensions.Primitives",
        "Microsoft.Maui.Graphics",
        "System.Diagnostics.DiagnosticSource",
        "System.Diagnostics.EventLog",
        "System.Runtime.CompilerServices.Unsafe",
        "System.Text.Encodings.Web",
        "System.Text.Json",
        "Xamarin.Android.Glide",
        "Xamarin.Android.Glide.DiskLruCache",
        "Xamarin.Android.Glide.GifDecoder",
        "Xamarin.AndroidX.Activity",
        "Xamarin.AndroidX.Annotation",
        "Xamarin.AndroidX.Annotation.Experimental",
        "Xamarin.AndroidX.AppCompat",
        "Xamarin.AndroidX.AppCompat.AppCompatResources",
        "Xamarin.AndroidX.Arch.Core.Common",
        "Xamarin.AndroidX.Arch.Core.Runtime",
        "Xamarin.AndroidX.AsyncLayoutInflater",
        "Xamarin.AndroidX.Browser",
        "Xamarin.AndroidX.CardView",
        "Xamarin.AndroidX.Collection",
        "Xamarin.AndroidX.Concurrent.Futures",
        "Xamarin.AndroidX.ConstraintLayout",
        "Xamarin.AndroidX.ConstraintLayout.Core",
        "Xamarin.AndroidX.CoordinatorLayout",
        "Xamarin.AndroidX.Core",
        "Xamarin.AndroidX.CursorAdapter",
        "Xamarin.AndroidX.CustomView",
        "Xamarin.AndroidX.DocumentFile",
        "Xamarin.AndroidX.DrawerLayout",
        "Xamarin.AndroidX.DynamicAnimation",
        "Xamarin.AndroidX.ExifInterface",
        "Xamarin.AndroidX.Fragment",
        "Xamarin.AndroidX.Interpolator",
        "Xamarin.AndroidX.Legacy.Support.Core.UI",
        "Xamarin.AndroidX.Legacy.Support.Core.Utils",
        "Xamarin.AndroidX.Legacy.Support.V4",
        "Xamarin.AndroidX.Lifecycle.Common",
        "Xamarin.AndroidX.Lifecycle.LiveData",
        "Xamarin.AndroidX.Lifecycle.LiveData.Core",
        "Xamarin.AndroidX.Lifecycle.Runtime",
        "Xamarin.AndroidX.Lifecycle.ViewModel",
        "Xamarin.AndroidX.Lifecycle.ViewModelSavedState",
        "Xamarin.AndroidX.Loader",
        "Xamarin.AndroidX.LocalBroadcastManager",
        "Xamarin.AndroidX.Media",
        "Xamarin.AndroidX.Navigation.Common",
        "Xamarin.AndroidX.Navigation.Fragment",
        "Xamarin.AndroidX.Navigation.Runtime",
        "Xamarin.AndroidX.Navigation.UI",
        "Xamarin.AndroidX.Print",
        "Xamarin.AndroidX.RecyclerView",
        "Xamarin.AndroidX.SavedState",
        "Xamarin.AndroidX.SlidingPaneLayout",
        "Xamarin.AndroidX.SwipeRefreshLayout",
        "Xamarin.AndroidX.Tracing.Tracing",
        "Xamarin.AndroidX.Transition",
        "Xamarin.AndroidX.VectorDrawable",
        "Xamarin.AndroidX.VectorDrawable.Animated",
        "Xamarin.AndroidX.VersionedParcelable",
        "Xamarin.AndroidX.ViewPager",
        "Xamarin.AndroidX.ViewPager2",
        "Xamarin.Google.Android.Material",
        "Xamarin.Google.Guava.ListenableFuture",
        "Mono.Android"
    };

    private static readonly string[] frameworkNames = new[]
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

    static NugetCommands()
    {
        if (!Directory.Exists(NugetDirectory))
        {
            Directory.CreateDirectory(NugetDirectory);
        }
    }

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
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<PackageMetadataResource>();

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
            var repository = Repository.Factory.GetCoreV3("https://nuget.cdn.azure.cn/v3/index.json");
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            using var packageStream = new MemoryStream();

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
            var zip = new ZipArchive(packageStream);
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
                    Console.WriteLine("正在下载缺少的包...");
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

                if (group.TargetFramework is null)
                {
                    throw new FrameworkNotFoundException(dependence.Key);
                }


                var frameworkDir = Path.Combine(dir, "lib", group.TargetFramework.GetShortFolderName());


                var libFiles = Directory.GetFiles(frameworkDir);

                var packageInfo = new PackageInfo
                {
                    Name = dependence.Key,
                    Version = dependence.Value.ToString(),
                };

                foreach (var f in libFiles)
                {
                    if (f.EndsWith(".dll"))
                    {
                        packageInfo.Paths.Add(f);
                    }
                }
                result.Add(packageInfo);
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
                    else if (!excludeDlls.Contains(p.Id))
                    {
                        dependencyGroup.Add(p.Id, version);
                    }
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

    public static async Task<NuGetVersion> ParseVersion(string pkgId, string version)
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
