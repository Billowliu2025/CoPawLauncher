using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

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
        
        // 在后台启动 copaw 进程
        StartCopawBackground();
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
