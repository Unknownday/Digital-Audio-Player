using Musical_Player.Config_management;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Net;

namespace Musical_Player.LoadingLogic
{
    public static class SetupLogic
    {
        /// <summary>
        /// Configures the program based on saved settings
        /// </summary>
        public static void ConfigureProgram()
        {
            // Try loading the configuration settings
            var currentConfig = ConfigManager.TryLoadConfig();

            if (currentConfig != null)
            {
                // Check and update DefaultPath if available
                if (currentConfig.TryGetValue("DefaultPath", out string defPath))
                {
                    if (!string.IsNullOrEmpty(defPath))
                    {
                        Config.DefaultPath = defPath;
                    }
                }

                // Check and update BackgroundImagePath if available
                if (currentConfig.TryGetValue("Background", out string background))
                {
                    if (!string.IsNullOrEmpty(background))
                    {
                        Config.BackgroundImagePath = background;
                    }
                }

                // Check and update LastPlaylist if available
                if (currentConfig.TryGetValue("LastPlaylist", out string playlist))
                {
                    if (!string.IsNullOrEmpty(playlist))
                    {
                        if (int.TryParse(playlist, out int playlistIndex))
                        {
                            Config.LastPlaylist = playlistIndex;
                        }
                    }
                }

                // Check and update LastSong if available
                if (currentConfig.TryGetValue("LastSong", out string song))
                {
                    if (!string.IsNullOrEmpty(song))
                    {
                        if (int.TryParse(song, out int songIndex))
                        {
                            Config.LastSong = songIndex;
                        }
                    }
                }

                // Check and update LastVolume if available
                if (currentConfig.TryGetValue("LastVolume", out string volume))
                {
                    if (!string.IsNullOrEmpty(volume))
                    {
                        if (int.TryParse(volume, out int lastVolume))
                        {
                            Config.LastVolume = lastVolume;
                        }
                    }
                }

                // Check and update AutoSwitching setting if available
                if (currentConfig.TryGetValue("AutoSwitch", out string autoSwitch))
                {
                    if (!string.IsNullOrEmpty(autoSwitch))
                    {
                        if (bool.TryParse(autoSwitch, out bool isAutoSwitch))
                        {
                            Player.IsAutoSwitching = isAutoSwitch;
                        }
                    }
                }

                // Check and update Theme if available
                if (currentConfig.TryGetValue("Theme", out string theme))
                {
                    if (!string.IsNullOrEmpty(theme))
                    {
                        Config.Theme = theme;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a dictionary of icon brushes based on the specified theme
        /// </summary>
        /// <param name="theme">Selected theme for icons</param>
        public static void CreateIconsBitmap(string theme)
        {
            Dictionary<int, ImageBrush> iconDictionary = new Dictionary<int, ImageBrush>();

            try
            {
                // Load the image from the specified path
                BitmapImage bitmapImage = new BitmapImage(new Uri(System.IO.Path.Combine(Config.DefaultPath, "Icons", $"{theme}IconSet.png")));

                int imageHeight = bitmapImage.PixelHeight;

                for (int i = 0; i < 13; i++)
                {
                    // Crop a 64x64 square from the image
                    CroppedBitmap croppedBitmap = new CroppedBitmap(
                        bitmapImage,
                        new Int32Rect(i * (imageHeight + 2), 0, imageHeight, imageHeight)
                    );

                    // Create an ImageBrush from the CroppedBitmap
                    ImageBrush imageBrush = new ImageBrush(croppedBitmap);

                    // Add to the dictionary
                    iconDictionary.Add(i + 1, imageBrush);
                }

                Config.IconsMap = iconDictionary;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
