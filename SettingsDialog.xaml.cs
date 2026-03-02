using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// SettingsDialog.xaml 的交互逻辑
/// </summary>
public partial class SettingsDialog : UserControl
{
    private string _currentColor = "Blue";
    private string? _customIconPath = null;

    public SettingsDialog()
    {
        InitializeComponent();
        
        // 默认高亮蓝色
        HighlightSelectedColor("Blue");
        
        // 加载当前主题状态
        LoadCurrentTheme();
        
        // 加载当前图标
        LoadCurrentIcon();
    }

    private void LoadCurrentTheme()
    {
        try
        {
            var isDark = ThemeManager.IsDarkTheme();
            if (isDark)
                DarkThemeRadio.IsChecked = true;
            else
                LightThemeRadio.IsChecked = true;
        }
        catch
        {
            DarkThemeRadio.IsChecked = true;
        }
    }

    private void LoadCurrentIcon()
    {
        try
        {
            // 检查是否有自定义图标
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var customIconPath = Path.Combine(appPath, "custom_icon.png");
            
            if (File.Exists(customIconPath))
            {
                _customIconPath = customIconPath;
                DisplayIconPreview(customIconPath);
                IconFilePathText.Text = $"当前图标：custom_icon.png";
            }
            else
            {
                // 使用默认图标
                IconFilePathText.Text = "当前图标：默认图标";
            }
        }
        catch
        {
            IconFilePathText.Text = "加载图标失败";
        }
    }

    private void DisplayIconPreview(string imagePath)
    {
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.DecodePixelWidth = 64;
            bitmap.DecodePixelHeight = 64;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            
            CurrentIconPreview.Source = bitmap;
        }
        catch
        {
            // 如果加载失败，不显示预览
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

    private void SelectIconButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "选择应用图标",
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp|PNG 文件|*.png|JPG 文件|*.jpg;*.jpeg|所有文件|*.*",
            FilterIndex = 1,
            CheckFileExists = true,
            CheckPathExists = true
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                var selectedPath = openFileDialog.FileName;
                
                // 验证图片尺寸
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                
                if (bitmap.Width < 64 || bitmap.Height < 64)
                {
                    MessageBox.Show(
                        "图片尺寸过小，建议使用 256x256 或更大的图片。\n\n" +
                        $"当前尺寸：{bitmap.Width}x{bitmap.Height}",
                        "警告",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                
                // 复制图片到应用目录
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var destPath = Path.Combine(appPath, "custom_icon.png");
                
                // 转换为 PNG 格式
                ConvertToPng(selectedPath, destPath);
                
                // 更新预览
                _customIconPath = destPath;
                DisplayIconPreview(destPath);
                IconFilePathText.Text = $"已选择：{Path.GetFileName(selectedPath)}";
                
                MessageBox.Show(
                    "图标已更新！\n\n注意：需要重启应用程序才能看到完整效果。\n桌面快捷方式图标可能需要重新创建。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"选择图标失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private void ConvertToPng(string sourcePath, string destPath)
    {
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(sourcePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            
            // 创建编码转换器保存为 PNG
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            
            using (var stream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(stream);
            }
        }
        catch
        {
            // 如果转换失败，直接复制文件
            File.Copy(sourcePath, destPath, true);
        }
    }

    private void ResetDefaults_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // 恢复默认：深色主题 + 蓝色
            DarkThemeRadio.IsChecked = true;
            _currentColor = "Blue";
            
            HighlightSelectedColor("Blue");
            ThemeManager.SetDarkTheme();
            ThemeManager.SetPrimaryColor(ThemeManager.GetColorFromName("Blue"));
            
            // 删除自定义图标
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var customIconPath = Path.Combine(appPath, "custom_icon.png");
            if (File.Exists(customIconPath))
            {
                File.Delete(customIconPath);
                _customIconPath = null;
                IconFilePathText.Text = "当前图标：默认图标";
                CurrentIconPreview.Source = null;
            }
            
            MessageBox.Show("已恢复默认设置", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"恢复默认设置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
