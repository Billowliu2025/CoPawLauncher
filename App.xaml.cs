using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CoPawLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Process? _copawProcess;
    private static bool _isExiting = false;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // 初始化设置数据库并加载已保存的主题
        SettingsStore.Initialize();
        LoadSavedTheme();
        
        // 加载自定义图标
        LoadCustomIcon();
        
        // 在后台启动 copaw 进程
        StartCopawBackground();
    }

    /// <summary>
    /// 加载自定义图标（如果存在）
    /// </summary>
    private void LoadCustomIcon()
    {
        try
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var customIconPath = Path.Combine(appPath, "custom_icon.png");
            
            if (File.Exists(customIconPath))
            {
                // 设置窗口图标 - 在 MainWindow 加载后设置
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (MainWindow != null)
                        {
                            MainWindow.Icon = new System.Windows.Media.Imaging.BitmapImage(
                                new Uri(customIconPath, UriKind.Absolute));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"设置窗口图标失败：{ex.Message}");
                    }
                }));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载自定义图标失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 从 SQLite 加载上次保存的主题设置
    /// </summary>
    private static void LoadSavedTheme()
    {
        try
        {
            var isDark = SettingsStore.GetBool(SettingsStore.KeyIsDarkTheme, defaultValue: false);
            if (isDark)
                ThemeManager.SetDarkTheme(save: false);
            else
                ThemeManager.SetLightTheme(save: false);

            var colorName = SettingsStore.Get(SettingsStore.KeyPrimaryColor, "Blue");
            var color = ThemeManager.GetColorFromName(colorName);
            ThemeManager.SetPrimaryColor(color, save: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"加载保存的主题设置失败：{ex.Message}");
        }
    }

    private static void StartCopawBackground()
    {
        try
        {
            _copawProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "copaw",
                    Arguments = "app",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            _copawProcess.Start();
            Console.WriteLine("CoPaw app started in background.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"启动 CoPaw 失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 停止 Copaw 进程
    /// </summary>
    public static void StopCopawProcess()
    {
        if (_copawProcess != null && !_copawProcess.HasExited && !_isExiting)
        {
            _isExiting = true;
            try
            {
                _copawProcess.Kill();
                _copawProcess.WaitForExit(5000); // 等待最多 5 秒
                Console.WriteLine("CoPaw app stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止 CoPaw 时出错：{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 获取 Copaw 进程是否正在运行
    /// </summary>
    public static bool IsCopawRunning => _copawProcess != null && !_copawProcess.HasExited;

    protected override void OnExit(ExitEventArgs e)
    {
        // 退出时结束 copaw 进程
        StopCopawProcess();
        base.OnExit(e);
    }
}
