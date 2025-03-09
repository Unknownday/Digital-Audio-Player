using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Musical_Player.LoadingLogic;
using System.IO;

namespace Musical_Player.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Constructor for the SettingsWindow
        /// </summary>
        public SettingsWindow()
        {
            // Initialize window components
            InitializeComponent();

            // Set the state of the automatic song switching checkbox according to the current value in Player
            AutoSongSwitchingSelector.IsChecked = PlayerModel.IsAutoSwitching;
            SwitchThemeCheckbox.IsChecked = Config.Theme == "Black" ? true : false;

            // Set the window background if the image path is specified
            if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
            {
                SetWindowBackground(Config.BackgroundImagePath);
            }
            if (Config.BackgroundImagePath == "none")
            {
                SetWindowBackground(Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}Background.png"));
            }

            ResetBackgroundButton.Source = Config.IconsMap[6].ImageSource;

            var color = System.Drawing.Color.FromName(Config.Theme);
            SwitchBackgroundButton.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SwitchThemeCheckbox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            AutoSongSwitchingSelector.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            ChooseDefaultDirButton.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            ChooseDefaultDirButton.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SwitchBackgroundButton.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));

        }

        // Field to store the dialog result (OK button pressed or not)
        private bool Result = false;

        // Event handler for the SwitchBackgroundButton click event
        private void SwitchBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the method to set a new background image from the FileManager class
            FileManager.SetNewBackgroundImage();

            // Set DialogResult to true and close the window
            Result = true;
            Close();
        }

        // Event handler for the ChooseDefaultDirButton click event
        private void ChooseDefaultDirButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the method to set a new default directory path from the FileManager class
            FileManager.SetNewDirectoryPath();

            // Set DialogResult to true and close the window
            Result = true;
            Close();
        }

        // Event handler for the AutoSongSwitchingSelector click event
        private void AutoSongSwitchingSelector_Click(object sender, RoutedEventArgs e)
        {
            // Update the value of AutoSongSwitchingSelector to the opposite of the current state
            AutoSongSwitchingSelector.IsChecked = !PlayerModel.IsAutoSwitching;

            // Update the value of the IsAutoSwitching property in Player according to the checkbox state
            PlayerModel.IsAutoSwitching = !PlayerModel.IsAutoSwitching;
        }

        // Event handler for the CloseButton click event
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            Close();
        }

        // Event handler for the window closing event
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Set DialogResult according to the result
            DialogResult = Result;
        }

        // Method to set an image as the window background
        private void SetWindowBackground(string path)
        {
            // Create ImageBrush
            ImageBrush imageBrush = new ImageBrush();

            // Create a BitmapImage object to load the image
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));

            // Set the image in ImageBrush
            imageBrush.ImageSource = bitmapImage;

            imageBrush.Stretch = Stretch.UniformToFill;

            // Set ImageBrush as the background for the window
            this.Background = imageBrush;
        }

        // Event handler for the ResetBackgroundButton mouse down event
        private void ResetBackgroundButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Set the background value to none
            Config.BackgroundImagePath = "none";

            // Set DialogResult to true and close the window
            Result = true;
            Close();
        }

        // Event handler for the SwitchThemeCheckbox click event
        private void SwitchThemeCheckbox_Click(object sender, RoutedEventArgs e)
        {
            // Update the state of the SwitchThemeCheckbox to the opposite
            SwitchThemeCheckbox.IsChecked = !SwitchThemeCheckbox.IsChecked;

            // Update the theme configuration based on the checkbox state
            if (SwitchThemeCheckbox.IsChecked == true)
            {
                Config.Theme = "White";
            }
            else
            {
                Config.Theme = "Black";
            }

            // Recreate icons bitmap based on the updated theme
            //SetupLogic.CreateIconsBitmap();

            // Set DialogResult to true and close the window
            Result = true;
            Close();
        }

    }
}
