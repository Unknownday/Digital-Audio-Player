using Musical_Player.Files_management;
using Musical_Player.LoadingLogic;
using MusicalPlayer;
using System.Windows;
using Musical_Player.Global;

namespace Musical_Player.Views
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        /// <summary>
        /// Constructor for the LoadingWindow
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();
            Topmost = true;
        }

        // Event handler for the ContentRendered event of the window
        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            // Configure the program settings and perform necessary setup logic
            SetupLogic.ConfigureProgram();
            FileManager.ValidateDefaultPath(Config.DefaultPath);
            SetupLogic.CreateIconsBitmap(Config.Theme);

            // Create and show the main window
            MainWindow window = new MainWindow();
            this.Close();
            window.Show();
        }
    }
}
