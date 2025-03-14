﻿using Musical_Player.Config_management;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Windows;

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
                                PlayerModel.Instance.IsAutoSwitching = bool.Parse(value);
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
            var HARDCODEDCFG = ConfigManager.TryLoadConfig();
            Config.DefaultPath = HARDCODEDCFG["DefaultPath"];
            ThemeManager modelInitialization = ThemeManager.Instance;

        }
    }
}
