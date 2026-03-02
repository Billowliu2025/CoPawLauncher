using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// AboutDialog.xaml 的交互逻辑
/// </summary>
public partial class AboutDialog : UserControl
{
    public AboutDialog()
    {
        InitializeComponent();
        // 动态显示当前版本号
        VersionText.Text = $"版本 {UpdateChecker.CurrentVersion}";
    }

    /// <summary>
    /// 检查更新按钮点击事件
    /// </summary>
    private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        // 切换到检查中状态
        CheckUpdateButton.IsEnabled = false;
        CheckUpdateButton.Content = "检查中...";
        UpdateIcon.Visibility = Visibility.Collapsed;
        UpdateProgress.Visibility = Visibility.Visible;
        UpdateStatusText.Text = "正在连接 GitHub 检查更新...";
        UpdateDownloadLink.Visibility = Visibility.Collapsed;

        var result = await UpdateChecker.CheckForUpdateAsync();

        // 恢复按钮
        UpdateProgress.Visibility = Visibility.Collapsed;
        UpdateIcon.Visibility = Visibility.Visible;
        CheckUpdateButton.IsEnabled = true;
        CheckUpdateButton.Content = "检查更新";

        if (!result.IsSuccess)
        {
            // 检查失败
            UpdateIcon.Kind = PackIconKind.AlertCircleOutline;
            UpdateIcon.Foreground = FindResource("MaterialDesignValidationErrorBrush") as System.Windows.Media.Brush
                                    ?? UpdateIcon.Foreground;
            UpdateStatusText.Text = result.ErrorMessage ?? "检查更新失败";
        }
        else if (result.HasUpdate)
        {
            // 有新版本
            UpdateIcon.Kind = PackIconKind.ArrowUpBoldCircle;
            UpdateIcon.Foreground = FindResource("PrimaryHueMidBrush") as System.Windows.Media.Brush
                                    ?? UpdateIcon.Foreground;
            UpdateStatusText.Text = $"发现新版本 v{result.LatestVersion}（当前 v{UpdateChecker.CurrentVersion}）";

            // 显示下载链接
            var downloadUrl = !string.IsNullOrEmpty(result.DownloadUrl) ? result.DownloadUrl : result.ReleasePageUrl;
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                DownloadHyperlink.NavigateUri = new System.Uri(downloadUrl);
                DownloadLinkText.Text = downloadUrl.EndsWith(".msi", System.StringComparison.OrdinalIgnoreCase)
                    ? "下载安装包 (.msi)"
                    : "前往 GitHub Releases 页面下载";
                UpdateDownloadLink.Visibility = Visibility.Visible;
            }
        }
        else
        {
            // 已是最新
            UpdateIcon.Kind = PackIconKind.CheckCircle;
            UpdateIcon.Foreground = FindResource("PrimaryHueMidBrush") as System.Windows.Media.Brush
                                    ?? UpdateIcon.Foreground;
            UpdateStatusText.Text = $"当前已是最新版本 (v{result.LatestVersion})";
        }
    }

    /// <summary>
    /// 下载链接导航事件
    /// </summary>
    private void DownloadLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogHost.CloseDialogCommand.Execute(null, null);
    }

    private void GitHubLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        OpenUrl(e.Uri.AbsoluteUri);
        e.Handled = true;
    }

    /// <summary>
    /// 用默认浏览器打开 URL
    /// </summary>
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
}
