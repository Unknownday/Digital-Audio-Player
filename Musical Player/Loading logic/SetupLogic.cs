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
        public static void ConfigureProgram()
        {
            var currentConfig = ConfigManager.TryLoadConfig();

            if (currentConfig != null)
            {
                if (currentConfig.TryGetValue("DefaultPath", out string defPath))
                {
                    if (!string.IsNullOrEmpty(defPath))
                    {
                        Config.DefaultPath = defPath;
                    }
                }

                if (currentConfig.TryGetValue("Background", out string background))
                {
                    if (!string.IsNullOrEmpty(background))
                    {
                        Config.BackgroundImagePath = background;
                    }
                }

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
                if (currentConfig.TryGetValue("Theme", out string theme))
                {
                    if (!string.IsNullOrEmpty(theme))
                    {
                        Config.Theme = theme;
                    }
                }
            }
        }

        public static void CreateIconsBitmap(string theme)
        {
            Dictionary<int, ImageBrush> iconDictionary = new Dictionary<int, ImageBrush>();

            try
            {
                // Загрузка изображения
                BitmapImage bitmapImage = new BitmapImage(new Uri(System.IO.Path.Combine(Config.DefaultPath, "Icons", $"{theme}IconSet.png")));

                int imageHeight = bitmapImage.PixelHeight;

                for (int i = 0; i < 12; i++)
                {
                    // Вырезание квадрата 64x64 из изображения
                    CroppedBitmap croppedBitmap = new CroppedBitmap(
                        bitmapImage,
                        new Int32Rect(i * (imageHeight+2), 0, imageHeight, imageHeight)
                            
                    );
                    // Создание ImageBrush из CroppedBitmap
                    ImageBrush imageBrush = new ImageBrush(croppedBitmap);

                    // Добавление в словарь
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
