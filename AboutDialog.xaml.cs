using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CoPawLauncher;

/// <summary>
/// AboutDialog.xaml 的交互逻辑
/// </summary>
public partial class AboutDialog : UserControl
{
    public AboutDialog()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        DialogHost.CloseDialogCommand.Execute(null, null);
    }
}
