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
    /// <summary>
    /// Programm setup logic
    /// </summary>
    public static class SetupLogic
    {
        

        /// <summary>
        /// Creates a dictionary of icon brushes
        /// </summary>
        public static void CreateIconsBitmap()
        {
            Dictionary<int, ImageBrush> iconDictionary = new Dictionary<int, ImageBrush>();

            try
            {
                // Load the image from the specified path
                BitmapImage bitmapImage = new BitmapImage(new Uri(System.IO.Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}IconSet.png")));

                int imageHeight = bitmapImage.PixelHeight;

                for (int i = 0; i < (bitmapImage.PixelWidth / 66); i++)
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
