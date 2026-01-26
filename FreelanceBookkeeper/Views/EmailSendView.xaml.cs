using FreelanceBookkeeper.Configuration;
using FreelanceBookkeeper.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace FreelanceBookkeeper.Views
{
    public partial class EmailSendView : Window
    {
        public ObservableCollection<int> Years { get; set; }
        public List<MonthGroup> MonthGroups { get; set; }
        public string RecipientEmail { get; set; }
        
        public int? SelectedYear { get; private set; }
        public int? SelectedMonthGroup { get; private set; }
        public string FinalRecipientEmail { get; private set; }
        public bool WasSent { get; private set; }

        public EmailSendView(ObservableCollection<int> years, List<MonthGroup> monthGroups)
        {
            InitializeComponent();
            
            Years = years;
            MonthGroups = monthGroups;
            
            var config = Config.Load();
            RecipientEmail = config.Email;
            
            DataContext = this;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Si us plau, introdueix un correu electrònic destinatari", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedYear = YearComboBox.SelectedItem as int?;
            SelectedMonthGroup = (MonthGroupComboBox.SelectedItem as MonthGroup)?.Group;
            FinalRecipientEmail = EmailTextBox.Text.Trim();
            WasSent = true;
            
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasSent = false;
            Close();
        }
    }
}