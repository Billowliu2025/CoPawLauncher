using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// SettingsDialog.xaml 的交互逻辑
/// </summary>
public partial class SettingsDialog : UserControl
{
    private readonly PaletteHelper _paletteHelper;
    private string _currentColor = "Blue";

    public SettingsDialog()
    {
        InitializeComponent();
        _paletteHelper = new PaletteHelper();
        
        // 默认高亮蓝色
        HighlightSelectedColor("Blue");
    }

    private void HighlightSelectedColor(string colorName)
    {
        _currentColor = colorName;
        
        // 清除所有颜色边框
        ClearColorHighlights();

        // 高亮选中的颜色
        var border = FindName($"Color{colorName}") as Border;
        if (border != null)
        {
            border.BorderBrush = Brushes.White;
            border.BorderThickness = new Thickness(3);
            border.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 10,
                ShadowDepth = 2,
                Opacity = 0.5,
                Color = Colors.Black
            };
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
            var border = FindName($"Color{name}") as Border;
            if (border != null)
            {
                border.BorderThickness = new Thickness(0);
                border.Effect = null;
            }
        }
    }

    private void ChangeTheme(bool isDark)
    {
        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
        _paletteHelper.SetTheme(theme);
    }

    private void ChangeColor(string colorName)
    {
        var theme = _paletteHelper.GetTheme();
        
        // MaterialDesign 支持的颜色
        var color = colorName switch
        {
            "Red" => System.Windows.Media.Colors.Red,
            "Pink" => System.Windows.Media.Colors.Pink,
            "Purple" => System.Windows.Media.Colors.Purple,
            "DeepPurple" => System.Windows.Media.Colors.Purple,
            "Indigo" => System.Windows.Media.Colors.Indigo,
            "Blue" => System.Windows.Media.Colors.Blue,
            "LightBlue" => System.Windows.Media.Colors.DeepSkyBlue,
            "Cyan" => System.Windows.Media.Colors.Cyan,
            "Teal" => System.Windows.Media.Colors.Teal,
            "Green" => System.Windows.Media.Colors.Green,
            "LightGreen" => System.Windows.Media.Colors.LimeGreen,
            "Orange" => System.Windows.Media.Colors.Orange,
            _ => System.Windows.Media.Colors.Blue
        };

        theme.PrimaryLight = color;
        theme.PrimaryMid = color;
        theme.PrimaryDark = color;
        _paletteHelper.SetTheme(theme);
    }

    #region 主题切换事件

    private void DarkThemeRadio_Checked(object sender, RoutedEventArgs e)
    {
        ChangeTheme(true);
    }

    private void LightThemeRadio_Checked(object sender, RoutedEventArgs e)
    {
        ChangeTheme(false);
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
        ChangeColor(colorName);
    }

    #endregion

    private void ResetDefaults_Click(object sender, RoutedEventArgs e)
    {
        // 恢复默认：深色主题 + 蓝色
        DarkThemeRadio.IsChecked = true;
        HighlightSelectedColor("Blue");
        ChangeTheme(true);
        ChangeColor("Blue");
        
        MessageBox.Show("已恢复默认设置", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogHost.CloseDialogCommand.Execute(true, null);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogHost.CloseDialogCommand.Execute(false, null);
    }
}
