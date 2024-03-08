using Musical_Player.Config_management;
using Musical_Player.Files_management;
using Musical_Player.LoadingLogic;
using MusicalPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Musical_Player.Global;
using Microsoft.Win32;
using System.Security.Policy;
using System.Net;

namespace Musical_Player.Views
{
    /// <summary>
    /// Логика взаимодействия для LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            Topmost = true;

            SetupLogic.ConfigureProgram();
            FileManager.ValidateDefaultPath(Config.DefaultPath);
            SetupLogic.CreateIconsBitmap(Config.Theme);

            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void CheckVersion(object sender, RoutedEventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                if (key != null)
                {
                    var ver = key.GetValue("Version");
                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            string newestVer = client.DownloadString("https://github.com/Unknownday/C-Audio-Player/raw/main/UpdateInfo.txt");
                            if (newestVer != null && !newestVer.Equals(ver)) 
                            { 
                                
                            }
                        }
                        catch { }
                    }
                    
                }
            }
        }
    }
}
