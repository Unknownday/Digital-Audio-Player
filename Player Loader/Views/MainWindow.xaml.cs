using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Player_Loader.Logic;
using System.Diagnostics;

namespace Player_Loader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var cfg = LoadingManager.CreateConfigModel();

            LoadingManager.ValidateDefaultPath(cfg);

            string appPath = @"Musical Player.exe";

            string arguments = $"-voice {cfg.LastVolumeAmount} -song {cfg.LastSongIndex} -playlist {cfg.LastPlaylistIndex} -theme {cfg.Theme} -auto_switching {cfg.IsAutoSwitching}";

            Process.Start(appPath, arguments);

            this.Close();
        }
    }
}
