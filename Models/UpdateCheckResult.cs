namespace CoPawLauncher.Models;

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
