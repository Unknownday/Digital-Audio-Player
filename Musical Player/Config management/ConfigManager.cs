using Microsoft.Win32;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;

namespace Musical_Player.Config_management
{
    public static class ConfigManager
    {
        public static Dictionary<string, string> TryLoadConfig()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                if (key != null)
                {
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
                    result = null;
                }
                return result;
            }
        }
        
        public static void SaveConfig(int playlistIndex, int songIndex, int volume)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
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
