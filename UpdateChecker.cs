using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CoPawLauncher;

/// <summary>
/// 通过 GitHub Releases API 检查版本更新，支持下载和安装
/// </summary>
public static class UpdateChecker
{
    /// <summary>GitHub 仓库拥有者</summary>
    private const string Owner = "Billowliu2025";

    /// <summary>GitHub 仓库名</summary>
    private const string Repo = "CoPawLauncher";

    /// <summary>GitHub API 获取最新 Release</summary>
    private static readonly string LatestReleaseUrl =
        $"https://api.github.com/repos/{Owner}/{Repo}/releases/latest";

    /// <summary>GitHub Web 页面最新 Release（用于重定向获取版本号，不消耗 API 限额）</summary>
    private static readonly string LatestReleaseWebUrl =
        $"https://github.com/{Owner}/{Repo}/releases/latest";

    /// <summary>用于 API 请求的全局 HttpClient</summary>
    private static readonly HttpClient _httpClient = CreateHttpClient();

    /// <summary>
    /// 获取当前程序版本号（从程序集 InformationalVersion 读取）
    /// </summary>
    public static string CurrentVersion
    {
        get
        {
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            // 去掉可能的 "+commithash" 后缀
            if (version != null && version.Contains('+'))
                version = version[..version.IndexOf('+')];
            return version ?? "1.0.0";
        }
    }

    /// <summary>
    /// 检查是否有新版本可用
    /// 优先通过 GitHub 页面重定向获取版本号（无 API 限额），仅在需要下载详情时调用 API
    /// </summary>
    public static async Task<UpdateCheckResult> CheckForUpdateAsync()
    {
        try
        {
            if (!Version.TryParse(CurrentVersion, out var currentVersion))
                return UpdateCheckResult.Failed($"无法解析当前版本号：{CurrentVersion}");

            // 第一步：通过页面重定向快速获取最新版本号（不消耗 API 限额）
            var latestTag = await GetLatestTagViaRedirectAsync();

            if (latestTag != null)
            {
                var latestTagClean = latestTag.TrimStart('v', 'V');
                if (!Version.TryParse(latestTagClean, out var latestVersion))
                    return UpdateCheckResult.Failed($"无法解析版本号：{latestTag}");

                // 当前已是最新版本，直接返回
                if (latestVersion <= currentVersion)
                {
                    return new UpdateCheckResult
                    {
                        HasUpdate = false,
                        LatestVersion = latestTagClean
                    };
                }

                // 有新版本，尝试调用 API 获取下载详情
                var apiResult = await GetReleaseDetailsViaApiAsync(latestTagClean, currentVersion);
                return apiResult;
            }

            // 重定向方式失败，回退到 API 方式
            return await GetReleaseDetailsViaApiAsync(null, currentVersion);
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"检查更新网络请求失败：{ex.Message}");
            var message = ex.StatusCode != null
                ? $"服务器返回错误 ({ex.StatusCode})，请稍后重试"
                : "网络连接失败，请检查网络设置";
            return UpdateCheckResult.Failed(message);
        }
        catch (TaskCanceledException)
        {
            return UpdateCheckResult.Failed("请求超时，请稍后重试");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"检查更新失败：{ex.Message}");
            return UpdateCheckResult.Failed($"检查失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 通过 GitHub 页面重定向获取最新版本标签（不消耗 API 限额）
    /// 请求 /releases/latest 页面，服务端返回 302 重定向到 /releases/tag/vX.Y.Z
    /// </summary>
    private static async Task<string?> GetLatestTagViaRedirectAsync()
    {
        try
        {
            using var handler = new HttpClientHandler { AllowAutoRedirect = false };
            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"CoPawLauncher/{CurrentVersion}");

            var response = await client.GetAsync(LatestReleaseWebUrl);

            if (response.StatusCode is System.Net.HttpStatusCode.Redirect
                or System.Net.HttpStatusCode.MovedPermanently)
            {
                var location = response.Headers.Location?.ToString();
                if (location != null)
                {
                    // URL 格式：https://github.com/.../releases/tag/v1.0.5
                    var tagIndex = location.LastIndexOf("/tag/", StringComparison.Ordinal);
                    if (tagIndex >= 0)
                        return location[(tagIndex + 5)..];
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"重定向获取版本失败：{ex.Message}");
        }
        return null;
    }

    /// <summary>
    /// 通过 GitHub API 获取 Release 详细信息（包含下载链接等）
    /// </summary>
    /// <param name="knownLatestVersion">已知最新版本号（可为 null 表示需要 API 自行获取）</param>
    /// <param name="currentVersion">当前版本</param>
    private static async Task<UpdateCheckResult> GetReleaseDetailsViaApiAsync(
        string? knownLatestVersion, Version currentVersion)
    {
        try
        {
            var response = await _httpClient.GetAsync(LatestReleaseUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return UpdateCheckResult.Failed("尚未发布任何版本，请关注 GitHub Releases 页面");

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // API 限额用尽，但如果已知有新版本，可引导用户到 Release 页面
                if (knownLatestVersion != null)
                {
                    return new UpdateCheckResult
                    {
                        HasUpdate = true,
                        LatestVersion = knownLatestVersion,
                        ReleasePageUrl = $"https://github.com/{Owner}/{Repo}/releases/latest"
                    };
                }
                return UpdateCheckResult.Failed("GitHub API 请求频率超限，请稍后重试（约 1 分钟）");
            }

            response.EnsureSuccessStatusCode();

            var release = await response.Content.ReadFromJsonAsync<GitHubRelease>();
            if (release == null)
                return UpdateCheckResult.Failed("无法获取版本信息");

            var latestTag = release.TagName?.TrimStart('v', 'V') ?? "";
            if (!Version.TryParse(latestTag, out var latestVersion))
                return UpdateCheckResult.Failed($"无法解析版本号：{release.TagName}");

            if (latestVersion > currentVersion)
            {
                var msiAsset = release.Assets?.FirstOrDefault(a =>
                    a.Name?.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) == true);

                return new UpdateCheckResult
                {
                    HasUpdate = true,
                    LatestVersion = latestTag,
                    ReleaseNotes = release.Body ?? "",
                    DownloadUrl = msiAsset?.BrowserDownloadUrl ?? "",
                    DownloadFileName = msiAsset?.Name ?? "",
                    DownloadSize = msiAsset?.Size ?? 0,
                    ReleasePageUrl = release.HtmlUrl ?? ""
                };
            }

            return new UpdateCheckResult
            {
                HasUpdate = false,
                LatestVersion = latestTag
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            if (knownLatestVersion != null)
            {
                return new UpdateCheckResult
                {
                    HasUpdate = true,
                    LatestVersion = knownLatestVersion,
                    ReleasePageUrl = $"https://github.com/{Owner}/{Repo}/releases/latest"
                };
            }
            return UpdateCheckResult.Failed("GitHub API 请求频率超限，请稍后重试");
        }
    }

    /// <summary>
    /// 下载更新文件到临时目录
    /// </summary>
    /// <param name="downloadUrl">下载地址</param>
    /// <param name="fileName">文件名</param>
    /// <param name="progress">进度回调（0~100）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>下载后的本地文件路径</returns>
    public static async Task<string> DownloadUpdateAsync(
        string downloadUrl,
        string fileName,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var downloadDir = Path.Combine(SettingsStore.DataFolder, "updates");
        Directory.CreateDirectory(downloadDir);
        var localPath = Path.Combine(downloadDir, fileName);

        // 如果已有同名文件先删除
        if (File.Exists(localPath))
            File.Delete(localPath);

        using var response = await _httpClient.GetAsync(downloadUrl,
            HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[81920];
        long totalRead = 0;
        int bytesRead;
        int lastReportedPercent = -1;

        while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalRead += bytesRead;

            if (totalBytes > 0)
            {
                var percent = (int)(totalRead * 100 / totalBytes);
                if (percent != lastReportedPercent)
                {
                    lastReportedPercent = percent;
                    progress?.Report(percent);
                }
            }
        }

        progress?.Report(100);
        return localPath;
    }

    /// <summary>
    /// 启动 MSI 安装程序并退出当前应用
    /// </summary>
    /// <param name="msiPath">MSI 文件路径</param>
    public static void LaunchInstallerAndExit(string msiPath)
    {
        try
        {
            // 使用 msiexec 启动安装
            Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec",
                Arguments = $"/i \"{msiPath}\"",
                UseShellExecute = true
            });

            // 停止 CoPaw 后台进程并退出应用
            App.StopCopawProcess();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"启动安装程序失败：{ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 创建配置好的 HttpClient
    /// </summary>
    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10) // 下载大文件需要更长超时
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd($"CoPawLauncher/{CurrentVersion}");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
        return client;
    }
}

/// <summary>
/// 版本更新检查结果
/// </summary>
public class UpdateCheckResult
{
    /// <summary>是否有新版本</summary>
    public bool HasUpdate { get; init; }

    /// <summary>最新版本号</summary>
    public string LatestVersion { get; init; } = "";

    /// <summary>更新说明</summary>
    public string ReleaseNotes { get; init; } = "";

    /// <summary>MSI 下载地址</summary>
    public string DownloadUrl { get; init; } = "";

    /// <summary>下载文件名</summary>
    public string DownloadFileName { get; init; } = "";

    /// <summary>下载文件大小（字节）</summary>
    public long DownloadSize { get; init; }

    /// <summary>Release 页面地址</summary>
    public string ReleasePageUrl { get; init; } = "";

    /// <summary>错误信息（检查失败时）</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>检查是否成功</summary>
    public bool IsSuccess => ErrorMessage == null;

    /// <summary>创建失败结果</summary>
    public static UpdateCheckResult Failed(string message) => new()
    {
        HasUpdate = false,
        ErrorMessage = message
    };
}

/// <summary>
/// GitHub Release API 响应模型
/// </summary>
public class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("assets")]
    public List<GitHubAsset>? Assets { get; set; }
}

/// <summary>
/// GitHub Release Asset 模型
/// </summary>
public class GitHubAsset
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("browser_download_url")]
    public string? BrowserDownloadUrl { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }
}
