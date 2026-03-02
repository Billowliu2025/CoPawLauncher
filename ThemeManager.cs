using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// 全局主题管理器
/// </summary>
public static class ThemeManager
{
    private static readonly PaletteHelper _paletteHelper = new PaletteHelper();

    /// <summary>
    /// 应用深色主题
    /// </summary>
    /// <param name="save">是否持久化到数据库</param>
    public static void SetDarkTheme(bool save = true)
    {
        ApplyBaseTheme(true);
        if (save) SettingsStore.SetBool(SettingsStore.KeyIsDarkTheme, true);
    }

    /// <summary>
    /// 应用浅色主题
    /// </summary>
    /// <param name="save">是否持久化到数据库</param>
    public static void SetLightTheme(bool save = true)
    {
        ApplyBaseTheme(false);
        if (save) SettingsStore.SetBool(SettingsStore.KeyIsDarkTheme, false);
    }

    /// <summary>
    /// 应用主色调
    /// </summary>
    /// <param name="color">颜色</param>
    /// <param name="save">是否持久化到数据库</param>
    public static void SetPrimaryColor(Color color, bool save = true)
    {
        try
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetPrimaryColor(color);
            _paletteHelper.SetTheme(theme);

            if (save)
            {
                var colorName = GetColorNameFromColor(color);
                SettingsStore.Set(SettingsStore.KeyPrimaryColor, colorName);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"设置主色调失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 根据 Color 反查颜色名称
    /// </summary>
    private static string GetColorNameFromColor(Color color)
    {
        var colorNames = new[] {
            "Red", "Pink", "Purple", "DeepPurple",
            "Indigo", "Blue", "LightBlue", "Cyan",
            "Teal", "Green", "LightGreen", "Orange"
        };
        foreach (var name in colorNames)
        {
            if (GetColorFromName(name) == color) return name;
        }
        return "Blue";
    }

    /// <summary>
    /// 应用基础主题
    /// </summary>
    /// <param name="isDark">是否为深色主题</param>
    private static void ApplyBaseTheme(bool isDark)
    {
        try
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
            _paletteHelper.SetTheme(theme);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"应用基础主题失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 获取当前主题是否为深色
    /// </summary>
    /// <returns>是否为深色主题</returns>
    public static bool IsDarkTheme()
    {
        try
        {
            var theme = _paletteHelper.GetTheme();
            return theme.GetBaseTheme() == BaseTheme.Dark;
        }
        catch
        {
            return true; // 默认深色
        }
    }

    /// <summary>
    /// 从颜色名称获取颜色
    /// </summary>
    public static Color GetColorFromName(string colorName)
    {
        return colorName switch
        {
            "Red" => Color.FromRgb(244, 67, 54),
            "Pink" => Color.FromRgb(233, 30, 99),
            "Purple" => Color.FromRgb(156, 39, 176),
            "DeepPurple" => Color.FromRgb(103, 58, 183),
            "Indigo" => Color.FromRgb(63, 81, 181),
            "Blue" => Color.FromRgb(33, 150, 243),
            "LightBlue" => Color.FromRgb(3, 169, 244),
            "Cyan" => Color.FromRgb(0, 188, 212),
            "Teal" => Color.FromRgb(0, 150, 136),
            "Green" => Color.FromRgb(76, 175, 80),
            "LightGreen" => Color.FromRgb(139, 195, 74),
            "Orange" => Color.FromRgb(255, 152, 0),
            _ => Color.FromRgb(33, 150, 243)
        };
    }
}
