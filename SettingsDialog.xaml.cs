using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// SettingsDialog.xaml 的交互逻辑
/// </summary>
public partial class SettingsDialog : UserControl
{
    private string _currentColor = "Blue";

    /// <summary>
    /// 打开对话框时记录的原始状态，用于取消时回滚
    /// </summary>
    private readonly bool _originalIsDark;
    private readonly string _originalColor;

    public SettingsDialog()
    {
        InitializeComponent();

        // 记录当前状态用于取消回滚
        _originalIsDark = ThemeManager.IsDarkTheme();
        _originalColor = SettingsStore.Get(SettingsStore.KeyPrimaryColor, "Blue");

        // 加载已保存的颜色高亮
        HighlightSelectedColor(_originalColor);

        // 加载当前主题状态
        LoadCurrentTheme();
    }

    private void LoadCurrentTheme()
    {
        try
        {
            if (_originalIsDark)
                DarkThemeRadio.IsChecked = true;
            else
                LightThemeRadio.IsChecked = true;
        }
        catch
        {
            DarkThemeRadio.IsChecked = true;
        }
    }

    private void HighlightSelectedColor(string colorName)
    {
        _currentColor = colorName;
        
        // 清除所有颜色边框
        ClearColorHighlights();

        // 高亮选中的颜色
        try
        {
            var border = FindName($"Color{colorName}") as Border;
            if (border != null)
            {
                border.BorderBrush = Brushes.White;
                border.BorderThickness = new Thickness(3);
                border.Effect = new DropShadowEffect
                {
                    BlurRadius = 10,
                    ShadowDepth = 2,
                    Opacity = 0.5,
                    Color = Colors.Black
                };
            }
        }
        catch
        {
            // 忽略高亮失败
        }
    }

    private void ClearColorHighlights()
    {
        var colorNames = new[] { 
            "Red", "Pink", "Purple", "DeepPurple",
            "Indigo", "Blue", "LightBlue", "Cyan",
            "Teal", "Green", "LightGreen", "Orange"
        };

        foreach (var name in colorNames)
        {
            try
            {
                var border = FindName($"Color{name}") as Border;
                if (border != null)
                {
                    border.BorderThickness = new Thickness(0);
                    border.Effect = null;
                }
            }
            catch
            {
                // 忽略清除失败
            }
        }
    }

    #region 主题切换事件

    private void DarkThemeRadio_Checked(object sender, RoutedEventArgs e)
    {
        ThemeManager.SetDarkTheme();
    }

    private void LightThemeRadio_Checked(object sender, RoutedEventArgs e)
    {
        ThemeManager.SetLightTheme();
    }

    #endregion

    #region 颜色选择事件

    private void ColorRed_Click(object sender, MouseButtonEventArgs e) => SelectColor("Red");
    private void ColorPink_Click(object sender, MouseButtonEventArgs e) => SelectColor("Pink");
    private void ColorPurple_Click(object sender, MouseButtonEventArgs e) => SelectColor("Purple");
    private void ColorDeepPurple_Click(object sender, MouseButtonEventArgs e) => SelectColor("DeepPurple");
    private void ColorIndigo_Click(object sender, MouseButtonEventArgs e) => SelectColor("Indigo");
    private void ColorBlue_Click(object sender, MouseButtonEventArgs e) => SelectColor("Blue");
    private void ColorLightBlue_Click(object sender, MouseButtonEventArgs e) => SelectColor("LightBlue");
    private void ColorCyan_Click(object sender, MouseButtonEventArgs e) => SelectColor("Cyan");
    private void ColorTeal_Click(object sender, MouseButtonEventArgs e) => SelectColor("Teal");
    private void ColorGreen_Click(object sender, MouseButtonEventArgs e) => SelectColor("Green");
    private void ColorLightGreen_Click(object sender, MouseButtonEventArgs e) => SelectColor("LightGreen");
    private void ColorOrange_Click(object sender, MouseButtonEventArgs e) => SelectColor("Orange");

    private void SelectColor(string colorName)
    {
        HighlightSelectedColor(colorName);
        var color = ThemeManager.GetColorFromName(colorName);
        ThemeManager.SetPrimaryColor(color);
    }

    #endregion

    private void ResetDefaults_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 恢复默认：浅色主题 + 蓝色
            LightThemeRadio.IsChecked = true;
            _currentColor = "Blue";
            
            HighlightSelectedColor("Blue");
            ThemeManager.SetLightTheme();
            ThemeManager.SetPrimaryColor(ThemeManager.GetColorFromName("Blue"));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"恢复默认设置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // 主题和颜色已在切换时实时持久化，直接关闭
        DialogHost.CloseDialogCommand.Execute(true, null);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        // 回滚到打开对话框前的状态
        if (_originalIsDark)
            ThemeManager.SetDarkTheme();
        else
            ThemeManager.SetLightTheme();

        ThemeManager.SetPrimaryColor(ThemeManager.GetColorFromName(_originalColor));

        DialogHost.CloseDialogCommand.Execute(false, null);
    }
}
