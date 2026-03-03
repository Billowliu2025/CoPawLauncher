using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _isMaximized = false;

    /// <summary>
    /// WebView2 用户数据目录，存放在 %LOCALAPPDATA%\CoPawLauncher 中
    /// </summary>
    private static readonly string _webView2UserDataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoPawLauncher");

    public MainWindow()
    {
        InitializeComponent();
        ApplyCustomIcon();
        InitializeWebView();
        UpdateTitleBarForeground();
        
        // 订阅主题变化事件
        ThemeManager.ThemeChanged += OnThemeChanged;
        
        // 添加键盘快捷键处理
        KeyDown += MainWindow_KeyDown;
    }

    /// <summary>
    /// 主题变化时更新标题栏颜色
    /// </summary>
    private void OnThemeChanged(bool isDark)
    {
        UpdateTitleBarForeground();
    }

    /// <summary>
    /// 更新标题栏前景色（深色主题白色，浅色主题黑色）
    /// </summary>
    private void UpdateTitleBarForeground()
    {
        var foreground = ThemeManager.GetTitleBarForegroundBrush();
        TitleText.Foreground = foreground;
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

    #region 窗口拖动与调整大小

    /// <summary>
    /// 标题栏拖动移动窗口，双击最大化/还原
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            MaximizeButton_Click(sender, e);
        }
        else
        {
            // 如果当前是最大化状态，拖动时先还原
            if (_isMaximized)
            {
                var point = e.GetPosition(this);
                _isMaximized = false;
                this.WindowState = WindowState.Normal;
                MaximizeIcon.Kind = PackIconKind.WindowMaximize;
                // 让窗口跟随鼠标位置
                this.Left = point.X - (this.ActualWidth / 2);
                this.Top = point.Y - 15;
            }
            this.DragMove();
        }
    }

    // Win32 API 用于窗口边缘拖动调整大小
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_SYSCOMMAND = 0x0112;

    /// <summary>
    /// 边缘拖动调整窗口大小
    /// </summary>
    private void Resize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not System.Windows.Shapes.Rectangle rect) return;
        if (_isMaximized) return; // 最大化时不允许调整

        var hwnd = new WindowInteropHelper(this).Handle;
        // SC_SIZE 方向码：1=左 2=右 3=上 4=左上 5=右上 6=下 7=左下 8=右下
        int direction = rect.Name switch
        {
            "ResizeTop" => 3,
            "ResizeBottom" => 6,
            "ResizeLeft" => 1,
            "ResizeRight" => 2,
            "ResizeTopLeft" => 4,
            "ResizeTopRight" => 5,
            "ResizeBottomLeft" => 7,
            "ResizeBottomRight" => 8,
            _ => 0
        };

        if (direction > 0)
        {
            ReleaseCapture();
            SendMessage(hwnd, WM_SYSCOMMAND, (IntPtr)(0xF000 + direction), IntPtr.Zero);
        }
    }

    #endregion

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
            UpdateTitleBarForeground();
            StatusText.Text = "设置已保存";
        }
    }

    #endregion
}