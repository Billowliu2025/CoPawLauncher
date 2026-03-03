using System.Diagnostics;
using System.Windows;

namespace CoPawLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // 初始化设置数据库并加载已保存的主题
        SettingsStore.Initialize();
        LoadSavedTheme();
        
        // 在后台启动 copaw 进程
        ProcessManager.StartCopaw();
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

    /// <summary>
    /// 停止 Copaw 进程（供外部调用，委托给 ProcessManager）
    /// </summary>
    public static void StopCopawProcess() => ProcessManager.StopCopaw();

    /// <summary>
    /// 获取 Copaw 进程是否正在运行
    /// </summary>
    public static bool IsCopawRunning => ProcessManager.IsCopawRunning;

    protected override void OnExit(ExitEventArgs e)
    {
        // 退出时结束 copaw 进程
        ProcessManager.StopCopaw();
        base.OnExit(e);
    }
}
