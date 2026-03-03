using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher.Views;

/// <summary>
/// AboutDialog.xaml 的交互逻辑
/// </summary>
public partial class AboutDialog : UserControl
{
    /// <summary>缓存的更新检查结果（用于"下载更新"按钮）</summary>
    private UpdateCheckResult? _updateResult;

    /// <summary>下载取消令牌</summary>
    private CancellationTokenSource? _downloadCts;

    /// <summary>下载完成后的 MSI 文件路径</summary>
    private string? _downloadedMsiPath;

    public AboutDialog()
    {
        InitializeComponent();
        VersionText.Text = $"版本 {UpdateChecker.CurrentVersion}";
    }

    #region 版本更新

    /// <summary>
    /// 检查更新 / 下载更新 / 安装更新 —— 按钮复用
    /// </summary>
    private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var buttonText = CheckUpdateButton.Content?.ToString();

            if (buttonText == "安装更新" && !string.IsNullOrEmpty(_downloadedMsiPath))
            {
                // 第三阶段：安装
                await InstallUpdateAsync();
            }
            else if (buttonText == "下载更新" && _updateResult?.HasUpdate == true)
            {
                // 第二阶段：下载
                await DownloadUpdateAsync();
            }
            else
            {
                // 第一阶段：检查
                await CheckForUpdateAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"更新操作异常：{ex}");
            SetUpdateStatus(PackIconKind.AlertCircleOutline, $"操作失败：{ex.Message}", isError: true);
            ResetButton("检查更新");
            DownloadProgressPanel.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 第一阶段：检查 GitHub Releases 最新版本
    /// </summary>
    private async Task CheckForUpdateAsync()
    {
        SetChecking(true);
        UpdateStatusText.Text = "正在连接 GitHub 检查更新...";

        var result = await UpdateChecker.CheckForUpdateAsync();

        SetChecking(false);

        if (!result.IsSuccess)
        {
            SetUpdateStatus(PackIconKind.AlertCircleOutline, result.ErrorMessage ?? "检查更新失败", isError: true);
            ResetButton("重试");
        }
        else if (result.HasUpdate)
        {
            _updateResult = result;
            SetUpdateStatus(PackIconKind.ArrowUpBoldCircle,
                $"发现新版本 v{result.LatestVersion}（当前 v{UpdateChecker.CurrentVersion}）");

            if (!string.IsNullOrEmpty(result.DownloadUrl))
            {
                CheckUpdateButton.Content = "下载更新";
                CheckUpdateButton.Style = (Style)FindResource("MaterialDesignRaisedButton");
            }
            else
            {
                // 无 MSI 附件，引导到 Release 页面
                UpdateStatusText.Text += "\n暂无安装包，请前往 GitHub Releases 页面查看。";
                ResetButton("检查更新");
            }
        }
        else
        {
            SetUpdateStatus(PackIconKind.CheckCircle, $"当前已是最新版本 (v{result.LatestVersion})");
            ResetButton("检查更新");
        }
    }

    /// <summary>
    /// 第二阶段：下载 MSI 安装包
    /// </summary>
    private async Task DownloadUpdateAsync()
    {
        if (_updateResult == null) return;

        CheckUpdateButton.IsEnabled = false;
        CheckUpdateButton.Content = "下载中...";
        DownloadProgressPanel.Visibility = Visibility.Visible;
        DownloadProgressBar.Value = 0;

        var sizeMb = _updateResult.DownloadSize > 0
            ? $" ({_updateResult.DownloadSize / 1024.0 / 1024.0:F1} MB)"
            : "";
        DownloadProgressText.Text = $"正在下载{sizeMb}...";
        UpdateStatusText.Text = $"正在下载 v{_updateResult.LatestVersion} 安装包...";

        _downloadCts = new CancellationTokenSource();
        var progress = new Progress<int>(percent =>
        {
            DownloadProgressBar.Value = percent;
            DownloadProgressText.Text = $"下载进度：{percent}%{sizeMb}";
        });

        _downloadedMsiPath = await UpdateChecker.DownloadUpdateAsync(
            _updateResult.DownloadUrl,
            _updateResult.DownloadFileName,
            progress,
            _downloadCts.Token);

        // 下载完成
        DownloadProgressBar.Value = 100;
        DownloadProgressText.Text = "下载完成！";
        SetUpdateStatus(PackIconKind.CheckCircle, $"v{_updateResult.LatestVersion} 安装包已下载完成");

        CheckUpdateButton.Content = "安装更新";
        CheckUpdateButton.IsEnabled = true;
        CheckUpdateButton.Style = (Style)FindResource("MaterialDesignRaisedButton");
    }

    /// <summary>
    /// 第三阶段：启动 MSI 安装并退出应用
    /// </summary>
    private async Task InstallUpdateAsync()
    {
        if (string.IsNullOrEmpty(_downloadedMsiPath)) return;

        // 确认安装
        var confirm = MessageBox.Show(
            $"即将安装 CoPaw Launcher v{_updateResult?.LatestVersion}。\n\n" +
            "安装过程中程序将自动关闭，安装完成后请重新启动。\n\n确认开始安装吗？",
            "确认安装更新",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.Yes);

        if (confirm != MessageBoxResult.Yes) return;

        UpdateStatusText.Text = "正在启动安装程序...";
        CheckUpdateButton.IsEnabled = false;

        // 短暂延迟让用户看到状态
        await Task.Delay(500);

        UpdateChecker.LaunchInstallerAndExit(_downloadedMsiPath);
    }

    #endregion

    #region UI 辅助方法

    /// <summary>
    /// 设置检查中 / 检查完成的 UI 状态
    /// </summary>
    private void SetChecking(bool isChecking)
    {
        CheckUpdateButton.IsEnabled = !isChecking;
        if (isChecking)
        {
            CheckUpdateButton.Content = "检查中...";
            UpdateIcon.Visibility = Visibility.Collapsed;
            UpdateSpinner.Visibility = Visibility.Visible;
            DownloadProgressPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            UpdateSpinner.Visibility = Visibility.Collapsed;
            UpdateIcon.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// 设置更新状态图标和文字
    /// </summary>
    private void SetUpdateStatus(PackIconKind icon, string message, bool isError = false)
    {
        UpdateIcon.Kind = icon;
        UpdateStatusText.Text = message;

        try
        {
            if (isError)
                UpdateIcon.Foreground = Brushes.OrangeRed;
            else
                UpdateIcon.Foreground = (Brush)FindResource("PrimaryHueMidBrush");
        }
        catch
        {
            // FindResource 找不到时静默处理
        }
    }

    /// <summary>
    /// 重置按钮文字和样式
    /// </summary>
    private void ResetButton(string text)
    {
        CheckUpdateButton.Content = text;
        CheckUpdateButton.IsEnabled = true;
        try
        {
            CheckUpdateButton.Style = (Style)FindResource("MaterialDesignOutlinedButton");
        }
        catch
        {
            // 静默处理
        }
    }

    #endregion

    #region 通用事件

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _downloadCts?.Cancel();
        DialogHost.CloseDialogCommand.Execute(null, null);
    }

    private void GitHubLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
    }

    private static void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法打开浏览器：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion
}
