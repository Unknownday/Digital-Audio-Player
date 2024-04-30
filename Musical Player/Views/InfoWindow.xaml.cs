using Musical_Player.Global;
using System.Windows;

namespace Musical_Player.Views
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        /// <summary>
        /// InfoWindow constructor
        /// </summary>
        public InfoWindow()
        {
            InitializeComponent();

            // Set the content of the VersionLabel to the application version from the global configuration
            VersionLabel.Content = Config.VERSION;
        }

        // Event handler for the button click event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            Close();
        }
    }
}