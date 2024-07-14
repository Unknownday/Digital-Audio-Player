using Microsoft.Win32;
using Player_Loader.Logic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace Player_Loader.Logic
{
    public static class LoadingManager
    {
        /// <summary>
        /// Trying to load config
        /// </summary>
        /// <returns>If successful, returns a dictionary with config. Else returns null</returns>
        public static Dictionary<string, string> TryLoadConfig()
        {
            // Create a new dictionary to store configuration values
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Open the registry key for reading configuration settings
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                // Check if the key exists
                if (key != null)
                {
                    // Read and add each configuration value to the dictionary
                    result.Add("DefaultPath", key.GetValue("DefaultPath") as string);

                    result.Add("LastPlaylist", key.GetValue("LastPlaylist") as string);
                    result.Add("LastSong", key.GetValue("LastSong") as string);

                    result.Add("LastVolume", key.GetValue("LastVolume") as string);
                    result.Add("Background", key.GetValue("Background") as string);
                    result.Add("AutoSwitch", key.GetValue("AutoSwitch") as string);
                    result.Add("Theme", key.GetValue("Theme") as string);
                    result.Add("Version", key.GetValue("Version") as string);
                }
                else
                {
                    // If the key doesn't exist, set the result to null
                    result = null;
                }

                // Return the result dictionary
                return result;
            }
        }
        
        /// <summary>
        /// Configures the program based on saved settings
        /// </summary>
        public static ConfigModel CreateConfigModel()
        {
            // Try loading the configuration settings
            var currentConfig = TryLoadConfig();

            int lastSong = 0;
            int lastPlaylist = 0;
            int volumeBackup = 50;
            string defaultPath = "";
            string backgroundImage = "";
            string thema = "";
            bool autoswitch = true;

            bool isOk = true;

            if (currentConfig != null)
            {
                // Check and update DefaultPath if available
                if (currentConfig.TryGetValue("DefaultPath", out string defPath))
                {
                    if (!string.IsNullOrEmpty(defPath))
                    {
                        defaultPath = defPath;
                    }
                    else
                    {
                        isOk = false;
                    }
                }

                // Check and update BackgroundImagePath if available
                if (currentConfig.TryGetValue("Background", out string background))
                {
                    if (!string.IsNullOrEmpty(background))
                    {
                        backgroundImage = background;
                    }
                    else
                    {
                        isOk = false;
                    }
                }

                // Check and update LastPlaylist if available
                if (currentConfig.TryGetValue("LastPlaylist", out string playlist))
                {
                    if (!string.IsNullOrEmpty(playlist))
                    {
                        if (int.TryParse(playlist, out int playlistIndex))
                        {
                            lastPlaylist = playlistIndex;
                        }
                    }
                    else 
                    { 
                        isOk = false; 
                    }
                }

                // Check and update LastSong if available
                if (currentConfig.TryGetValue("LastSong", out string song))
                {
                    if (!string.IsNullOrEmpty(song))
                    {
                        if (int.TryParse(song, out int songIndex))
                        {
                            lastSong = songIndex;
                        }

                    }
                    else
                    {
                        isOk = false;
                    }
                }

                // Check and update LastVolume if available
                if (currentConfig.TryGetValue("LastVolume", out string volume))
                {
                    if (!string.IsNullOrEmpty(volume))
                    {
                        if (int.TryParse(volume, out int lastVolume))
                        {
                            volumeBackup = lastVolume;
                        }
                    }
                    else
                    {
                        isOk = false;
                    }
                }

                // Check and update AutoSwitching setting if available
                if (currentConfig.TryGetValue("AutoSwitch", out string autoSwitch))
                {
                    if (!string.IsNullOrEmpty(autoSwitch))
                    {
                        if (bool.TryParse(autoSwitch, out bool isAutoSwitch))
                        {
                            autoswitch = isAutoSwitch;
                        }
                    }
                    else
                    {
                        isOk = false;
                    }
                }

                // Check and update Theme if available
                if (currentConfig.TryGetValue("Theme", out string theme))
                {
                    if (!string.IsNullOrEmpty(theme))
                    {
                        thema = theme;
                    }
                    else
                    {
                        isOk = false;
                    }
                }
            }
            else
            {
                isOk = false;
            }

            if (!isOk)
            {
                InitDefaultRegistry();
                return null;
            }

            return new ConfigModel(lastSong, lastPlaylist, volumeBackup, defaultPath, backgroundImage, thema, autoswitch);
        }

        /// <summary>
        /// Checks if the required directory exists
        /// </summary>
        public static void ValidateDefaultPath(ConfigModel cfg)
        {
            // Create directories if they do not exist
            
            if (!Directory.Exists(cfg.DefaultPath))
            {
                Directory.CreateDirectory(cfg.DefaultPath);
            }

            if (!File.Exists(Path.Combine(cfg.DefaultPath, "Playlists.xml")))
            {
                XElement playlistsElement = new XElement("playlists");
                XDocument xDocument = new XDocument(playlistsElement);
                xDocument.Save(Path.Combine(cfg.DefaultPath, "Playlists.xml"));
            }

            if (!Directory.Exists(Path.Combine(cfg.DefaultPath, "Icons")))
            {
                Directory.CreateDirectory(Path.Combine(cfg.DefaultPath, "Icons"));
            }

            if (!File.Exists(Path.Combine(cfg.DefaultPath, "Icons", "BlackIconSet.png"))
                || !File.Exists(Path.Combine(cfg.DefaultPath, "Icons", "WhiteIconSet.png"))
                || !File.Exists(Path.Combine(cfg.DefaultPath, "Icons", "WhiteBackground.png"))
                || !File.Exists(Path.Combine(cfg.DefaultPath, "Icons", "BlackBackground.png")))
            {
                DownloadMiscFiles(cfg.DefaultPath);
            }
        }

        /// <summary>
        /// Downloads miscellaneous files
        /// </summary>
        public static void DownloadMiscFiles(string defaultPath)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Download files from URLs to the specified directory
                    client.DownloadFile("https://sharedby.blomp.com/Jrcdqf", Path.Combine(defaultPath, "Icons", "BlackIconSet.png"));
                    client.DownloadFile("https://sharedby.blomp.com/6nEIsE", Path.Combine(defaultPath, "Icons", "WhiteIconSet.png"));
                    client.DownloadFile("https://sharedby.blomp.com/6XyxQT", Path.Combine(defaultPath, "Icons", "BlackBackground.png"));
                    client.DownloadFile("https://sharedby.blomp.com/6dxQGn", Path.Combine(defaultPath, "Icons", "WhiteBackground.png"));
                }
                catch { }
            }
        }

        public static void InitDefaultRegistry()
        {
            // Open or create the registry key for writing configuration settings
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                // Set each configuration value in the registry
                key.SetValue("DefaultPath", @"C:\\Digital audio player");
                key.SetValue("LastPlaylist", "-1");
                key.SetValue("LastSong", "-1");
                key.SetValue("LastVolume", "50");
                key.SetValue("Background", "none");
                key.SetValue("AutoSwitch", true);
                key.SetValue("Theme", "White");
                key.SetValue("AppPath", AppDomain.CurrentDomain.BaseDirectory);
                key.SetValue("Version", "UNABLE TO GET DATA");
            }
        }
    }
}
