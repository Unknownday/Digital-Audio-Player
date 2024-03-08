using Microsoft.Win32;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;

namespace Musical_Player.Config_management
{
    /// <summary>
    /// Configuration management
    /// </summary>
    public static class ConfigManager
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
        /// Saving configuration
        /// </summary>
        /// <param name="playlistIndex">Chosen playlist index</param>
        /// <param name="songIndex">Chosen song index</param>
        /// <param name="volume">Current volume</param>
        public static void SaveConfig(int playlistIndex, int songIndex, int volume)
        {
            // Open or create the registry key for writing configuration settings
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                // Set each configuration value in the registry
                key.SetValue("DefaultPath", Config.DefaultPath);
                key.SetValue("LastPlaylist", playlistIndex.ToString());
                key.SetValue("LastSong", songIndex.ToString());
                key.SetValue("LastVolume", volume.ToString());
                key.SetValue("Background", Config.BackgroundImagePath);
                key.SetValue("AutoSwitch", Player.IsAutoSwitching.ToString());
                key.SetValue("Theme", Config.Theme);
                key.SetValue("AppPath", AppDomain.CurrentDomain.BaseDirectory);
                key.SetValue("Version", Config.VERSION);
            }
        }
    }
}
