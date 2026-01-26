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
        LoadConfiguration();
        SetWindowSize();
    }

    private void LoadConfiguration()
    {
        MonthsTextBox.Text = config.MonthsToShow.ToString();
        EmailTextBox.Text = config.Email;
        SmtpServerTextBox.Text = config.SmtpServer;
        SmtpPortTextBox.Text = config.SmtpPort.ToString();
        SmtpUsernameTextBox.Text = config.SmtpUsername;
        SmtpPasswordBox.Password = config.SmtpPassword;
        SmtpUseSslCheckBox.IsChecked = config.SmtpUseSsl;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(MonthsTextBox.Text, out int months) && months > 0)
        {
            config.MonthsToShow = months;
            config.Email = EmailTextBox.Text.Trim();
            config.SmtpServer = SmtpServerTextBox.Text.Trim();

            if (int.TryParse(SmtpPortTextBox.Text, out int port))
                config.SmtpPort = port;

            config.SmtpUsername = SmtpUsernameTextBox.Text.Trim();
            config.SmtpPassword = SmtpPasswordBox.Password;
            config.SmtpUseSsl = SmtpUseSslCheckBox.IsChecked ?? true;

            config.Save();
            MessageBox.Show("Configuració guardada");
            Close();
        }
        else
        {
            MessageBox.Show("Introdueix un número vàlid per als mesos");
        }
    }

    private void SetWindowSize()
    {
        var screen = System.Windows.SystemParameters.WorkArea;
        this.Width = screen.Width * 0.3;
        this.Height = screen.Height * 0.5;
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }
}
