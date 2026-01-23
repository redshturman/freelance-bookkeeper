using FreelanceBookkeeper.Configuration;
using System.Windows;

namespace FreelanceBookkeeper.Views;

public partial class SettingsView : Window
{
    private Config config;

    public SettingsView()
    {
        InitializeComponent();
        config = Config.Load();
        MonthsTextBox.Text = config.MonthsToShow.ToString();
        SetWindowSize();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(MonthsTextBox.Text, out int months) && months > 0)
        {
            config.MonthsToShow = months;
            config.Save();
            MessageBox.Show("Configuració guardada");
            Close();
        }
        else
        {
            MessageBox.Show("Introdueix un número vàlid");
        }
    }

    private void SetWindowSize()
    {
        var screen = System.Windows.SystemParameters.WorkArea;
        this.Width = screen.Width * 0.3;
        this.Height = screen.Height * 0.4;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }
}
