using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;

namespace CoPawLauncher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeWebView();
        
        // 添加键盘快捷键处理
        KeyDown += MainWindow_KeyDown;
    }

    private async void InitializeWebView()
    {
        try
        {
            // 初始化 WebView2
            await webView.EnsureCoreWebView2Async(null);
            
            // 设置导航事件
            webView.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    statusText.Text = "CoPaw 已就绪";
                }
                else
                {
                    statusText.Text = $"加载失败：{args.WebErrorStatus}";
                }
            };

            // 允许开发者工具（可选）
            webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            
            // 导航到 CoPaw 聊天页面
            webView.CoreWebView2.Navigate("http://127.0.0.1:8088/chat");
        }
        catch (Exception ex)
        {
            statusText.Text = $"初始化失败：{ex.Message}";
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
            ExitButton_Click(null, null);
            e.Handled = true;
        }
        // Esc 也可以退出
        else if (e.Key == Key.Escape)
        {
            ExitButton_Click(null, null);
            e.Handled = true;
        }
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs? e)
    {
        if (webView.CoreWebView2 != null)
        {
            webView.CoreWebView2.Reload();
            statusText.Text = "正在刷新...";
        }
    }

    private void ExitButton_Click(object? sender, RoutedEventArgs? e)
    {
        // 确认退出
        var result = MessageBox.Show(
            "确定要退出 CoPaw Launcher 吗？\n\n这将同时停止后台运行的 CoPaw 服务。",
            "确认退出",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
            // 更新状态
            statusText.Text = "正在退出...";
            
            // 停止 Copaw 进程
            App.StopCopawProcess();
            
            // 关闭主窗口
            Application.Current.Shutdown();
        }
    }
}
