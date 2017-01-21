using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Communicator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            login.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(login.Text))
            {
                var mdSettings = new MetroDialogSettings()
                {
                    ColorScheme = MetroDialogColorScheme.Accented,
                };
                this.ShowMessageAsync("Brak loginu", "Login nie może być pusty", MessageDialogStyle.Affirmative, mdSettings);
            }
            else
            {
                var chatWindow = new ChatWindow(login.Text);
                chatWindow.Show();
                Close();
            }
        }
    }
}
