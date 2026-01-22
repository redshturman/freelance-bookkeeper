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
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(MonthsTextBox.Text, out int months) && months > 0)
        {
            config.MonthsToShow = months;
            config.Save();
            MessageBox.Show("Configuración guardada");
            Close();
        }
        else
        {
            MessageBox.Show("Introduce un número válido");
        }
    }
}
