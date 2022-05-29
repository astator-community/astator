using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System.Data;
using Settings = NuGet.Configuration.Settings;

namespace astator.NugetManager;

public class NugetCommands
{
    private static readonly string packagesDir = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory("astator").ToString(), "nuget");

    private static readonly string globalConfigPath = Path.Combine(packagesDir, "nuget.config");

    private static readonly PackagePathResolver packagePathResolver = new(packagesDir);

    private static readonly NuGetFramework nuGetFramework = NuGetFramework.Parse("net6.0-android31.0");

    static NugetCommands()
    {
        if (!Directory.Exists(packagesDir))
        {
            Directory.CreateDirectory(packagesDir);
        }
        if (!File.Exists(globalConfigPath))
        {
            File.WriteAllText(globalConfigPath,
               @"<?xml version=""1.0"" encoding =""utf-8"" ?>
<configuration>
  <packageSources>
    <add key=""huaweicloud"" value =""https://repo.huaweicloud.com/repository/nuget/v3/index.json"" />
    <add key=""nuget.org"" value =""https://api.nuget.org/v3/index.json"" />
  </packageSources>
</configuration>
");
        }
    }

    /// <summary>
    /// 搜索包
    /// </summary>
    /// <returns></returns>
    public static async Task<List<IPackageSearchMetadata>> SearchPkgAsync(string pkgId, string nugetSource)
    {
        return await Task.Run(async () =>
        {
            using var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(nugetSource);
            var resource = await repository.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrerelease: true);

            var results = await resource.SearchAsync(
            pkgId,
            searchFilter,
            skip: 0,
            take: 15,
            NullLogger.Instance,
            new CancellationTokenSource(30000).Token);

            return results.ToList();
        });
    }

    /// <summary>
    /// 获取包的元数据
    /// </summary>
    /// <returns></returns>
    public static async Task<List<IPackageSearchMetadata>> GetPackagesMetadataAsync(string pkgId, string nugetSource)
    {
        return await Task.Run(async () =>
        {
            using var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(nugetSource);
            var resource = await repository.GetResourceAsync<PackageMetadataResource>();

            var packages = await resource.GetMetadataAsync(
                pkgId,
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                NullLogger.Instance,
                new CancellationTokenSource(30000).Token);

            var result = packages.ToList();
            result.Reverse();
            return result;
        });
    }

    public static NuGetFramework GetNearestFramework(IEnumerable<NuGetFramework> targetFrameworks)
    {
        return new FrameworkReducer().GetNearest(nuGetFramework, targetFrameworks);
    }

    public static string GetInstalledDir(string pkgId, NuGetVersion version)
    {
        var packageIdentity = new PackageIdentity(pkgId, version);
        return packagePathResolver.GetInstalledPath(packageIdentity);
    }

    private static void GetPackageDependencies(PackageIdentity package,
               NuGetFramework framework,
               SourceCacheContext cacheContext,
               ILogger logger,
               IEnumerable<SourceRepository> repositories,
               ref HashSet<SourcePackageDependencyInfo> availablePackages)
    {
        if (availablePackages.Contains(package)) return;

        foreach (var repository in repositories)
        {
            var dependencyInfoResource = repository.GetResourceAsync<DependencyInfoResource>().Result;
            var dependencyInfo = dependencyInfoResource.ResolvePackage(
                package, framework, cacheContext, logger, CancellationToken.None).Result;

            if (dependencyInfo == null) continue;

            availablePackages.Add(dependencyInfo);
            foreach (var dependency in dependencyInfo.Dependencies)
            {
                GetPackageDependencies(
                   new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                   framework, cacheContext, logger, repositories, ref availablePackages);
            }
            break;
        }
    }

    private static async Task<IEnumerable<string>> ExtractPackageAsync(
       string source,
       Stream packageStream,
       PackagePathResolver packagePathResolver,
       PackageExtractionContext packageExtractionContext,
       CancellationToken token,
       Guid parentId = default)
    {
        var packageSaveMode = packageExtractionContext.PackageSaveMode;
        var filesAdded = new List<string>();
        var nupkgStartPosition = packageStream.Position;

        var packageExtractionTelemetryEvent = new PackageExtractionTelemetryEvent(packageExtractionContext.PackageSaveMode, NuGetOperationStatus.Failed, ExtractionSource.NuGetFolderProject);
        using (var telemetry = TelemetryActivity.Create(parentId, packageExtractionTelemetryEvent))
        {
            using (var packageReader = new PackageArchiveReader(packageStream, leaveStreamOpen: true))
            {
                var packageIdentityFromNuspec = await packageReader.GetIdentityAsync(CancellationToken.None);
                packageExtractionTelemetryEvent.LogPackageIdentity(packageIdentityFromNuspec);

                var installPath = packagePathResolver.GetInstallPath(packageIdentityFromNuspec);
                var packageDirectoryInfo = Directory.CreateDirectory(installPath);
                var packageDirectory = packageDirectoryInfo.FullName;

                try
                {
                    telemetry.StartIntervalMeasure();

                    telemetry.EndIntervalMeasure(PackagingConstants.PackageVerifyDurationName);
                }
                catch (SignatureException)
                {
                    throw;
                }

                var packageFiles = await packageReader.GetPackageFilesAsync(packageSaveMode, token);

                if ((packageSaveMode & PackageSaveMode.Nuspec) == PackageSaveMode.Nuspec)
                {
                    var sourceNuspecFile = packageFiles.Single(p => PackageHelper.IsManifest(p));

                    var targetNuspecPath = Path.Combine(
                        packageDirectory,
                        packagePathResolver.GetManifestFileName(packageIdentityFromNuspec));

                    // Extract the .nuspec file with a well known file name.
                    filesAdded.Add(packageReader.ExtractFile(
                        sourceNuspecFile,
                        targetNuspecPath,
                        packageExtractionContext.Logger));

                    packageFiles = packageFiles.Except(new[] { sourceNuspecFile });
                }

                var packageFileExtractor = new PackageFileExtractor(packageFiles, packageExtractionContext.XmlDocFileSaveMode);

                filesAdded.AddRange(await packageReader.CopyFilesAsync(
                    packageDirectory,
                    packageFiles,
                    packageFileExtractor.ExtractPackageFile,
                    packageExtractionContext.Logger,
                    token));

                if ((packageSaveMode & PackageSaveMode.Nupkg) == PackageSaveMode.Nupkg)
                {
                    // During package extraction, nupkg is the last file to be created
                    // Since all the packages are already created, the package stream is likely positioned at its end
                    // Reset it to the nupkgStartPosition
                    packageStream.Seek(nupkgStartPosition, SeekOrigin.Begin);

                    var nupkgFilePath = Path.Combine(
                        packageDirectory,
                        packagePathResolver.GetPackageFileName(packageIdentityFromNuspec));

                    filesAdded.Add(packageStream.CopyToFile(nupkgFilePath));
                }

                // Now, copy satellite files unless requested to not copy them
                if (packageExtractionContext.CopySatelliteFiles)
                {
                    filesAdded.AddRange(await CopySatelliteFilesAsync(
                        packageReader,
                        packagePathResolver,
                        packageSaveMode,
                        packageExtractionContext,
                        token));
                }

                packageExtractionTelemetryEvent.SetResult(NuGetOperationStatus.Succeeded);
            }

            return filesAdded;
        }
    }

    private static async Task<IEnumerable<string>> CopySatelliteFilesAsync(
           PackageReaderBase packageReader,
           PackagePathResolver packagePathResolver,
           PackageSaveMode packageSaveMode,
           PackageExtractionContext packageExtractionContext,
           CancellationToken token)
    {
        if (packageReader == null)
        {
            throw new ArgumentNullException(nameof(packageReader));
        }

        if (packagePathResolver == null)
        {
            throw new ArgumentNullException(nameof(packagePathResolver));
        }

        if (packageExtractionContext == null)
        {
            throw new ArgumentNullException(nameof(packageExtractionContext));
        }

        var satelliteFilesCopied = Enumerable.Empty<string>();

        var result = await PackageHelper.GetSatelliteFilesAsync(packageReader, packagePathResolver, token);

        var runtimePackageDirectory = result.Item1;
        var satelliteFiles = result.Item2
            .Where(file => PackageHelper.IsPackageFile(file, packageSaveMode))
            .ToList();

        if (satelliteFiles.Count > 0)
        {
            var packageFileExtractor = new PackageFileExtractor(satelliteFiles, packageExtractionContext.XmlDocFileSaveMode);

            // Now, add all the satellite files collected from the package to the runtime package folder(s)
            satelliteFilesCopied = await packageReader.CopyFilesAsync(
                runtimePackageDirectory,
                satelliteFiles,
                packageFileExtractor.ExtractPackageFile,
                packageExtractionContext.Logger,
                token);
        }

        return satelliteFilesCopied;
    }

    private readonly ISettings settings;

    public NugetCommands(string userConfigPath = null)
    {
        var configFilePaths = new List<string> { globalConfigPath };
        if (userConfigPath is not null && File.Exists(userConfigPath)) configFilePaths.Add(userConfigPath);

        this.settings = Settings.LoadSettingsGivenConfigPaths(new List<string> { globalConfigPath });
    }

    /// <summary>
    /// 安装包
    /// </summary>
    /// <returns></returns>
    public async Task<bool> InstallPackageAsync(string pkgId, NuGetVersion version)
    {
        return await Task.Run(async () =>
        {
            var result = new List<string>();
            var packageSourceProvider = new PackageSourceProvider(this.settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());
            using var cacheContext = new SourceCacheContext();
            var repositories = sourceRepositoryProvider.GetRepositories();

            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            GetPackageDependencies(new PackageIdentity(pkgId, version), nuGetFramework, cacheContext, NullLogger.Instance, repositories, ref availablePackages);

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { pkgId },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                NullLogger.Instance);
            var packageIdentitys = new PackageResolver().Resolve(resolverContext, CancellationToken.None)
                .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
            var nnn = ClientPolicyContext.GetClientPolicy(NullSettings.Instance, NullLogger.Instance);
            var packageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Defaultv3,
                XmlDocFileSaveMode.None,
                ClientPolicyContext.GetClientPolicy(this.settings, NullLogger.Instance),
                NullLogger.Instance);
            var frameworkReducer = new FrameworkReducer();

            foreach (var packageIdentity in packageIdentitys)
            {
                var installedPath = packagePathResolver.GetInstalledPath(packageIdentity);
                if (installedPath == null)
                {
                    var downloadResource = await packageIdentity.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                    var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                        packageIdentity,
                        new PackageDownloadContext(cacheContext),
                        SettingsUtility.GetGlobalPackagesFolder(this.settings),
                        NullLogger.Instance,
                        new CancellationTokenSource(60000).Token);

                    if (downloadResult.Status == DownloadResourceResultStatus.NotFound || downloadResult.Status == DownloadResourceResultStatus.Cancelled)
                    {
                        throw new InstallPackageException(pkgId);
                    }
                    await ExtractPackageAsync(
                         downloadResult.PackageSource,
                         downloadResult.PackageStream,
                         packagePathResolver,
                         packageExtractionContext,
                         CancellationToken.None);
                }
            }
            return true;
        });
    }

    /// <summary>
    /// 解析包版本
    /// </summary>
    /// <param name="pkgId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public async Task<NuGetVersion> ParseVersionAsync(string pkgId, string version)
    {
        if (!version.EndsWith("*")) return NuGetVersion.Parse(version);
        return await Task.Run(async () =>
        {
            var versions = new List<NuGetVersion>();
            var packageSourceProvider = new PackageSourceProvider(this.settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());
            using var cacheContext = new SourceCacheContext();
            var repositories = sourceRepositoryProvider.GetRepositories();
            var cache = new SourceCacheContext();
            foreach (var repository in repositories)
            {
                var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
                versions = (await resource.GetAllVersionsAsync(pkgId, cache, NullLogger.Instance, CancellationToken.None)).ToList();
                if (versions is not null && versions.Any()) break;
            }
            return versions.FindBestMatch(VersionRange.Parse(version), version => version);
        });
    }

    /// <summary>
    /// 还原包
    /// </summary>
    /// <param name="pkgId"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public async Task<PackageInfo> RestorePackageAsync(string pkgId, NuGetVersion version)
    {
        return await Task.Run(async () =>
        {
            var result = new PackageInfo
            {
                Name = pkgId,
                Version = version.ToString()
            };
            var packageSourceProvider = new PackageSourceProvider(this.settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());
            using var cacheContext = new SourceCacheContext();
            var repositories = sourceRepositoryProvider.GetRepositories();
            var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

            GetPackageDependencies(new PackageIdentity(pkgId, version), nuGetFramework, cacheContext, NullLogger.Instance, repositories, ref availablePackages);

            var resolverContext = new PackageResolverContext(
                DependencyBehavior.Lowest,
                new[] { pkgId },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                sourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                NullLogger.Instance);
            var packageIdentitys = new PackageResolver().Resolve(resolverContext, CancellationToken.None)
                .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
            var packageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Defaultv3,
                XmlDocFileSaveMode.None,
                ClientPolicyContext.GetClientPolicy(NullSettings.Instance, new NullLogger()),
                NullLogger.Instance);

            foreach (var packageIdentity in packageIdentitys)
            {
                PackageReaderBase packageReader;
                var installedPath = packagePathResolver.GetInstalledPath(packageIdentity);
                if (installedPath == null)
                {
                    var downloadResource = await packageIdentity.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                    var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                        packageIdentity,
                        new PackageDownloadContext(cacheContext),
                        SettingsUtility.GetGlobalPackagesFolder(this.settings),
                        NullLogger.Instance, CancellationToken.None);

                    if (downloadResult.Status == DownloadResourceResultStatus.NotFound || downloadResult.Status == DownloadResourceResultStatus.Cancelled)
                    {
                        continue;
                    }

                    await PackageExtractor.ExtractPackageAsync(
                        downloadResult.PackageSource,
                        downloadResult.PackageStream,
                        packagePathResolver,
                        packageExtractionContext,
                        CancellationToken.None);

                    packageReader = downloadResult.PackageReader;
                }
                else
                {
                    packageReader = new PackageFolderReader(installedPath);
                }

                var libItems = packageReader.GetLibItems();
                var nearest = new FrameworkReducer().GetNearest(nuGetFramework, libItems.Select(x => x.TargetFramework));
                var libs = libItems.Where(x => x.TargetFramework.Equals(nearest)).SelectMany(x => x.Items).ToList();
                if (libs.Any())
                {
                    foreach (var lib in libs)
                    {
                        if (lib.EndsWith(".dll")) result.Compile.Add(Path.Combine(packagePathResolver.GetInstalledPath(packageIdentity), lib));
                    }
                }
            }
            return result;
        });
    }

    public List<SourceRepository> GetRepositories()
    {
        var packageSourceProvider = new PackageSourceProvider(this.settings);
        var sourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, Repository.Provider.GetCoreV3());
        return sourceRepositoryProvider.GetRepositories().ToList();
    }

}
