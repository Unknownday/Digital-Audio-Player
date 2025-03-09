using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace Musical_Player.Files_management
{
    /// <summary>
    /// File management
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// File name filter. Cleans the file name from unnecessary elements
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>Cleaned name</returns>
        public static string NameFilter(string name)
        {
            // List of file formats to be removed from the name
            List<string> DeletingFormats = new List<string>()
            {
                ".mp3",
                ".wav",
                ".ogg",
                ".playlist",
            };

            // Iterating through each format and removing it from the name
            foreach (string to_replace in DeletingFormats)
            {
                name = name.Replace(to_replace, "");
            }
            return name; // Returning the filtered name
        }

        /// <summary>
        /// Checks if a song exists at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if the song exists. False if the song does not exist</returns>
        public static bool ValidatePath(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Checks if the song format is supported for Drag and Drop
        /// </summary>
        /// <param name="filePath">Path to the song to check</param>
        /// <returns>True if the format is supported. False if the format is not supported</returns>
        public static bool IsSupportedAudioFormat(string filePath)
        {
            // Supported audio formats
            string[] supportedFormats = { ".mp3", ".wav", ".ogg" };
            string fileExtension = Path.GetExtension(filePath);

            // Check if the file extension is in the list of supported formats
            return supportedFormats.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets a new default directory path
        /// </summary>
        public static void SetNewDirectoryPath()
        {
            // Open a directory selection dialog
            var dialog = new FolderBrowserDialog();
            DialogResult dialogResult = dialog.ShowDialog();

            // If a new directory is selected, update the configuration
            if (DialogResult.OK == dialogResult)
            {
                var newDefaultPath = dialog.SelectedPath;
                var files = Directory.GetFiles(Config.DefaultPath, "*.playlist");

                // Copy existing playlist files to the new directory and update the configuration
                foreach (var file in files)
                {
                    File.Copy(file, Path.Combine(newDefaultPath, Path.GetFileName(file)));
                    File.Delete(file);
                }

                foreach (var image in Directory.GetFiles(Path.Combine(Config.DefaultPath, "Icons")))
                {
                    var imageFile = File.ReadAllLines(image);
                    File.WriteAllLines(Path.Combine(newDefaultPath, "Icons", Path.GetFileName(image)), imageFile);
                }

                Config.DefaultPath = newDefaultPath;
            }
        }

        /// <summary>
        /// Sets a new background image
        /// </summary>
        public static void SetNewBackgroundImage()
        {
            // Open a file dialog to choose a new background image
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(png),(jpg)|*.png;*.jpg;";

            // If a new image is chosen, update the configuration
            if (opnFileDlg.ShowDialog() == true && opnFileDlg.FileNames.Length != 0)
            {
                Config.BackgroundImagePath = opnFileDlg.FileNames[0];
            }
        }

        /// <summary>
        /// Creates a dictionary of icon brushes
        /// </summary>
        public static List<ImageBrush> CreateIconsBitmap(string theme)
        {
            

            try
            {
                List<ImageBrush> icons = new List<ImageBrush>();
                // Load the image from the specified path
                BitmapImage bitmapImage = new BitmapImage(new Uri(System.IO.Path.Combine(Config.DefaultPath, "Icons", $"{theme}IconSet.png")));

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
                    icons.Add(imageBrush);
                }

                return icons;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "1322123");
                return null;
            }
        }
    }
}
