using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MusicalPlayer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// On startup event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0)
            {
                for (int i = 0; i < e.Args.Length; i++)
                {
                    if (i < e.Args.Length - 1)
                    {
                        string key = e.Args[i];
                        string value = e.Args[i + 1];

                        switch (key)
                        {
                            case "-voice":
                                Config.LastVolume = Convert.ToInt32(value);
                                break;
                            case "-song":
                                Config.LastSong = Convert.ToInt32(value);
                                break;
                            case "-playlist":
                                Config.LastPlaylist = Convert.ToInt32(value);
                                break;
                            case "-auto_switching":
                                PlayerModel.IsAutoSwitching = bool.Parse(value);
                                break;
                            case "-theme":
                                Config.Theme = value;
                                break;
                            default:

                                break;
                        }
                    }
                }
            }
            else
            {
                
            }
        }
    }
}
