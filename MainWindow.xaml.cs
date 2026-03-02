using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _isMaximized = false;

    /// <summary>
    /// WebView2 用户数据目录，存放在 %LOCALAPPDATA%\CoPawLauncher 下
    /// </summary>
    private static readonly string _webView2UserDataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoPawLauncher");

    public MainWindow()
    {
        InitializeComponent();
        ApplyCustomIcon();
        InitializeWebView();
        
        // 添加键盘快捷键处理
        KeyDown += MainWindow_KeyDown;
    }

    /// <summary>
    /// 加载并应用自定义图标（如果存在）
    /// </summary>
    public void ApplyCustomIcon()
    {
        try
        {
            var iconFile = SettingsStore.Get(SettingsStore.KeyCustomIconFile);
            if (string.IsNullOrEmpty(iconFile)) return;

            var iconPath = Path.Combine(SettingsStore.DataFolder, iconFile);
            if (!File.Exists(iconPath)) return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            Icon = bitmap;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载自定义图标失败：{ex.Message}");
        }
    }

    private async void InitializeWebView()
    {
        try
        {
            // 初始化 WebView2，指定用户数据目录
            var env = await CoreWebView2Environment.CreateAsync(null, _webView2UserDataFolder);
            await WebView.EnsureCoreWebView2Async(env);
            
            // 设置导航事件
            WebView.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    StatusText.Text = "CoPaw 已就绪";
                    StatusIcon.Kind = PackIconKind.CheckCircle;
                }
                else
                {
                    StatusText.Text = $"加载失败：{args.WebErrorStatus}";
                    StatusIcon.Kind = PackIconKind.AlertCircle;
                }
            };

            // 允许开发者工具（可选）
            WebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            
            // 导航到 CoPaw 聊天页面
            WebView.CoreWebView2.Navigate("http://127.0.0.1:8088/chat");
        }
        catch (Exception ex)
        {
            StatusText.Text = $"初始化失败：{ex.Message}";
            StatusIcon.Kind = PackIconKind.AlertCircle;
            MessageBox.Show($"WebView2 初始化失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        // F5 刷新
        if (e.Key == Key.F5)
        {
            RefreshButton_Click(null, null);
            e.Handled = true;
        }
        // Alt+F4 退出（默认已支持，这里显式处理）
        else if (e.Key == Key.F4 && Keyboard.Modifiers == ModifierKeys.Alt)
        {
            Close();
            e.Handled = true;
        }
        // Esc 关闭菜单
        else if (e.Key == Key.Escape)
        {
            MainMenuPopup.IsOpen = false;
            e.Handled = true;
        }
    }

    #region 窗口控制按钮

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isMaximized)
        {
            this.WindowState = WindowState.Normal;
            MaximizeIcon.Kind = PackIconKind.WindowMaximize;
            _isMaximized = false;
        }
        else
        {
            this.WindowState = WindowState.Maximized;
            MaximizeIcon.Kind = PackIconKind.WindowRestore;
            _isMaximized = true;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // 默认关闭窗口，不退出进程
        SoftExitButton_Click(sender, e);
    }

    #endregion

    #region 菜单功能

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        MainMenuPopup.IsOpen = !MainMenuPopup.IsOpen;
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs? e)
    {
        MainMenuPopup.IsOpen = false;
        
        if (WebView.CoreWebView2 != null)
        {
            WebView.CoreWebView2.Reload();
            StatusText.Text = "正在刷新...";
            StatusIcon.Kind = PackIconKind.Refresh;
        }
    }

    private void SoftExitButton_Click(object? sender, RoutedEventArgs? e)
    {
        MainMenuPopup.IsOpen = false;
        // 仅关闭窗口，不退出 copaw 进程
        this.Close();
    }

    private async void ForceExitButton_Click(object? sender, RoutedEventArgs? e)
    {
        MainMenuPopup.IsOpen = false;
        
        // 确认退出
        var result = MessageBox.Show(
            "确定要强制退出 CoPaw Launcher 吗？\n\n这将同时停止后台运行的 CoPaw 服务。",
            "确认强制退出",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
            // 更新状态
            StatusText.Text = "正在退出...";
            StatusIcon.Kind = PackIconKind.Loading;
            
            // 停止 Copaw 进程
            App.StopCopawProcess();
            
            // 稍微延迟以显示状态
            await Task.Delay(500);
            
            // 关闭主窗口
            Application.Current.Shutdown();
        }
    }

    private async void AboutButton_Click(object? sender, RoutedEventArgs? e)
    {
        MainMenuPopup.IsOpen = false;
        
        var aboutDialog = new AboutDialog();
        await DialogHost.Show(aboutDialog, "RootDialog");
    }

    private async void SettingsButton_Click(object? sender, RoutedEventArgs? e)
    {
        MainMenuPopup.IsOpen = false;
        
        var settingsDialog = new SettingsDialog();
        var result = await DialogHost.Show(settingsDialog, "RootDialog");
        
        // 处理保存结果：立即应用图标
        if (result is bool saved && saved)
        {
            ApplyCustomIcon();
            StatusText.Text = "设置已保存";
        }
    }

    #endregion
}
