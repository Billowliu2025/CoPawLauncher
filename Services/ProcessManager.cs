using System.Diagnostics;
using System.Windows;

namespace CoPawLauncher.Services;

/// <summary>
/// CoPaw 后台进程管理器
/// 负责启动、停止和监控 copaw app 进程的生命周期
/// </summary>
public static class ProcessManager
{
    private static Process? _copawProcess;
    private static bool _isExiting = false;

    /// <summary>
    /// 获取 CoPaw 进程是否正在运行
    /// </summary>
    public static bool IsCopawRunning => _copawProcess != null && !_copawProcess.HasExited;

    /// <summary>
    /// 在后台启动 copaw app 进程
    /// </summary>
    public static void StartCopaw()
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
            Debug.WriteLine("CoPaw app started in background.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"启动 CoPaw 失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 停止 CoPaw 后台进程
    /// </summary>
    public static void StopCopaw()
    {
        if (_copawProcess != null && !_copawProcess.HasExited && !_isExiting)
        {
            _isExiting = true;
            try
            {
                _copawProcess.Kill();
                _copawProcess.WaitForExit(5000); // 等待最多 5 秒
                Debug.WriteLine("CoPaw app stopped.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"停止 CoPaw 时出错：{ex.Message}");
            }
        }
    }
}
