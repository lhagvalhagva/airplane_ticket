using System.Configuration;
using System.Data;
using System.Windows;
using AirplaneTicket.WPF.Pages;

namespace AirplaneTicket.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new Window
        {
            Title = "Онгоцны зорчигч бүртгэл, мэдээлэх систем",
            Width = 1200,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
    }
}