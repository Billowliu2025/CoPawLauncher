using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CoPawLauncher;

/// <summary>
/// 通过 GitHub Releases API 检查版本更新
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
    /// </summary>
    /// <returns>检查结果</returns>
    public static async Task<UpdateCheckResult> CheckForUpdateAsync()
    {
        try
        {
            // 先发送请求，检查响应状态码
            var response = await _httpClient.GetAsync(LatestReleaseUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return UpdateCheckResult.Failed("尚未发布任何版本，请关注 GitHub Releases 页面");

            response.EnsureSuccessStatusCode();

            var release = await response.Content.ReadFromJsonAsync<GitHubRelease>();
            if (release == null)
                return UpdateCheckResult.Failed("无法获取版本信息");

            var latestTag = release.TagName?.TrimStart('v', 'V') ?? "";
            if (!Version.TryParse(latestTag, out var latestVersion))
                return UpdateCheckResult.Failed($"无法解析版本号：{release.TagName}");

            if (!Version.TryParse(CurrentVersion, out var currentVersion))
                return UpdateCheckResult.Failed($"无法解析当前版本号：{CurrentVersion}");

            if (latestVersion > currentVersion)
            {
                // 查找 MSI 下载链接
                var msiAsset = release.Assets?.FirstOrDefault(a =>
                    a.Name?.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) == true);

                return new UpdateCheckResult
                {
                    HasUpdate = true,
                    LatestVersion = latestTag,
                    ReleaseNotes = release.Body ?? "",
                    DownloadUrl = msiAsset?.BrowserDownloadUrl ?? release.HtmlUrl ?? "",
                    ReleasePageUrl = release.HtmlUrl ?? ""
                };
            }

            return new UpdateCheckResult
            {
                HasUpdate = false,
                LatestVersion = latestTag
            };
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
    /// 创建配置好的 HttpClient
    /// </summary>
    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        // GitHub API 要求 User-Agent
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

    /// <summary>下载地址（优先 MSI，fallback 到 Release 页面）</summary>
    public string DownloadUrl { get; init; } = "";

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
