using Musical_Player.Config_management;
using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Musical_Player.Global.Enums;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;

namespace MusicalPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Variable to track playlist deletion
        /// </summary>
        private bool isDeleted = false;

        /// <summary>
        /// Variable to track playlist selection
        /// </summary>
        private bool isPlaylistChoosed = false;

        /// <summary>
        /// Variable to track song selection
        /// </summary>
        private bool isSongChoosed = false;

        /// <summary>
        /// Timer for updating the interface
        /// </summary>
        private Timer durationTimer;

        /// <summary>
        /// Constructor for the MainWindow class
        /// </summary>
        public MainWindow()
        {
            // Initialize program components
            InitializeComponent();

            UpdateUi();

            // Set window background if an image path is specified
            if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
            {
                SetWindowBackground(Config.BackgroundImagePath);
            }
            if (Config.BackgroundImagePath == "none")
            {
                SetWindowBackground(Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}Background.png"));
            }

            // Allow file dragging
            AllowDrop = true;

            // Attach function to the "file drop" event
            Drop += MainWindow_Drop;

            // Check the existence of the default playlist path
            FileManager.ValidateDefaultPath(Config.DefaultPath);

            // Update the playlist list
            UpdatePlaylistList();

            // Create a timer for updating the interface
            durationTimer = new Timer();

            // Attach event handler for timer updates
            durationTimer.Tick += new EventHandler(UpdateDuration);

            // Set the timer interval in milliseconds
            durationTimer.Interval = 1;

            // Start the timer
            durationTimer.Start();

            // Set the value for the volume label
            VolumeLabel.Content = $"{Config.LastVolume}%";

            // Set the value for the volume slider
            VolumeSlider.Value = Config.LastVolume;

            // Restore the previous playlist
            PlaylistsList.SelectedIndex = Config.LastPlaylist;

            Player.CurrentSongVolume = (float)(Config.LastVolume / 100);

            // Restore the previous song
            SongList.SelectedIndex = Config.LastSong;
        }

        /// <summary>
        /// Event handler for the "Drop" event when dragging files into the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            // Hide the tooltip
            TipLabel.Visibility = Visibility.Hidden;

            // Check if a playlist is selected
            if (!isPlaylistChoosed) { MessageBox.Show("Select a playlist first!", "Musical player"); return; }

            // Get the list of files dragged into the window
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check if each file is a supported audio format
            if (files.Any(file => !FileManager.IsSupportedAudioFormat(file))) { return; }

            // Add each supported audio file to the selected playlist
            files.Where(FileManager.IsSupportedAudioFormat).ToList().ForEach(file => FileManager.AddSong(file, PlaylistsList.SelectedItem.ToString()));

            // Update the song list in the interface
            UpdateSongList();
        }

        /// <summary>
        /// Event handler for mouse click on the program info button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgramInfoButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Create and show the information window
            var infoWindow = new InfoWindow();
            infoWindow.Show();
        }

        /// <summary>
        /// Event handler for mouse click on the mute button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuteButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Save the current volume
                var tempSoundValue = Player.CurrentSongVolume;

                // Mute the sound
                Player.CurrentSongVolume = Player.AudioValueBackup;

                // Save the current volume for restoration
                Player.AudioValueBackup = tempSoundValue;

                // Update the player state
                Player.IsMuted = !Player.IsMuted;

                // Set the value for the volume slider
                VolumeSlider.Value = Player.CurrentSongVolume * 100;

                // Set the value for the volume label
                VolumeLabel.Content = $"{Convert.ToInt32(Math.Abs(Player.CurrentSongVolume * 100))}%";

                // Set the new volume value for the player
                Player.AudioPlayer.Volume = Player.CurrentSongVolume;
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the add playlist button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPlaylistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Create a new playlist
                FileManager.CreatePlaylist();

                // Update the playlist list
                UpdatePlaylistList();
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the delete playlist button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePlaylistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Delete the selected playlist
                FileManager.DeletePlaylist(PlaylistsList.SelectedIndex);

                isSongChoosed = false;
                isPlaylistChoosed = false;
                isDeleted = true;
                PlaylistsList.SelectedIndex = -1;

                // Update the playlist list
                UpdatePlaylistList();
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the move up button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveUpButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                if (currentIndex - 1 < 0) { return; }
                // Move the song up in the queue
                FileManager.MoveSongInQueue(SongList.SelectedIndex, PlaylistsList.SelectedIndex, Enums.MoveDirections.Up);

                // Update the song list
                UpdateSongList();

                SongList.SelectedIndex = currentIndex - 1;
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the move down button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                if (currentIndex + 1 >= SongList.Items.Count) { return; }
                // Move the song down in the queue
                FileManager.MoveSongInQueue(currentIndex, PlaylistsList.SelectedIndex, Enums.MoveDirections.Down);

                // Update the song list
                UpdateSongList();

                SongList.SelectedIndex = currentIndex + 1;
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the add song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist is selected
            if (!isPlaylistChoosed) { return; }

            try
            {
                // Add a song to the playlist
                FileManager.AddSongToPlaylist(PlaylistsList.SelectedIndex);

                // Update the song list
                UpdateSongList();
            }
            catch (Exception ex)
            {
                // Display an error message
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for mouse click on the delete song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                var currentIndex = SongList.SelectedIndex;
                // Delete the selected song from the playlist
                FileManager.DeleteSong(PlaylistsList.SelectedIndex, currentIndex);

                // Update the song list
                UpdateSongList();

                SongList.SelectedIndex = currentIndex;
            }
            catch { }
        }

        /// <summary>
        /// Event handler for mouse click on the play next song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayNextSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Get the index of the next song
                var nextIndex = SongList.SelectedIndex += 1;

                // Check if the index goes beyond the song list boundaries
                if (nextIndex >= SongList.Items.Count) { SongList.SelectedIndex = 0; return; }

                // Set the index for the next song
                SongList.SelectedIndex = nextIndex;

                // Initialize and play the song
                InitSong(SongList.SelectedIndex);

                // Update the maximum slider length
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                PlaySong();
            }
            catch { }
        }

        /// <summary>
        /// Mouse click handler for the pause button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and a song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Update the current player state
                Player.CurrentPlayerState = PlayerStates.OnPause;

                // Pause playback
                Player.AudioPlayer.Pause();
            }
            catch { }
        }

        /// <summary>
        /// Mouse click handler for the play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and a song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Depending on the current player state, perform the corresponding actions
                switch (Player.CurrentPlayerState)
                {
                    case PlayerStates.Idle:
                    case PlayerStates.InProgress:
                        // Initialize the song
                        InitSong(SongList.SelectedIndex);

                        // Set the maximum value of the duration slider equal to the song duration
                        DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                        // Play the song
                        PlaySong();

                        // Update the label with the song name
                        SongNameLabel.Content = Player.CurrentSongName;

                        // Start the interface update timer
                        durationTimer.Start();
                        break;

                    case PlayerStates.OnPause:
                        // Update the player state
                        Player.CurrentPlayerState = PlayerStates.InProgress;

                        // Resume playback of the song
                        Player.AudioPlayer.Play();

                        // Start the interface update timer
                        durationTimer.Start();
                        break;
                }
            }
            catch { }
        }


        /// <summary>
        /// Mouse click handler for the play previous song button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPreviosSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check if a playlist and a song are selected
            if (!isPlaylistChoosed || !isSongChoosed) { return; }

            try
            {
                // Get the index of the next song
                var nextIndex = SongList.SelectedIndex -= 1;

                // Check if the index goes beyond the song list boundaries
                if (nextIndex < 0) { SongList.SelectedIndex = SongList.Items.Count - 1; return; }

                // Set the index of the next song
                SongList.SelectedIndex = nextIndex;

                // Initialize and play the song
                InitSong(SongList.SelectedIndex);

                // Update the maximum length of the slider
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

                PlaySong();
            }
            catch { }
        }

        /// <summary>
        /// Selection changed event handler for updating the selected playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Check if a playlist has been deleted
                if (!isDeleted)
                {
                    // Check if a playlist is selected
                    if (PlaylistsList.SelectedIndex == -1) return;

                    // Set the playlist selection flag
                    isPlaylistChoosed = true;

                    // Update the song list in the interface
                    UpdateSongList();
                }
                if (isDeleted)
                {
                    SongList.ItemsSource = null;
                    isDeleted = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// Event handler for updating the song duration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDuration(object sender, EventArgs e)
        {
            try
            {
                // Check if the current song is set
                if (Player.CurrentSong != null)
                {
                    // Update the label with the duration
                    DurationLabel.Content = PlayerLogic.GetDurationLabel(Player.CurrentSong.CurrentTime.Minutes,
                            Player.CurrentSong.CurrentTime.Seconds,
                            Player.CurrentSong.TotalTime.Minutes,
                            Player.CurrentSong.TotalTime.Seconds
                        );

                    // Set the value of the duration slider
                    DurationSlider.Value = Player.CurrentSong.CurrentTime.Seconds + 60 * Player.CurrentSong.CurrentTime.Minutes;

                    // Display the current status in the window title
                    string playbackState = Player.CurrentPlayerState == PlayerStates.InProgress ? "▶" : "||";
                    Title = Player.IsAutoSwitching ? $"{playbackState} {Player.CurrentSongName} |⇌|" : $"{playbackState} {Player.CurrentSongName}";

                    // Update the label with the song name
                    SongNameLabel.Content = Player.CurrentSongName;

                    // Update the label with the volume
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(Player.CurrentSongVolume * 100))}%";
                }
            }
            catch { }
        }

        /// <summary>
        /// Method for playing the next song considering auto-switching permission
        /// </summary>
        /// <param name="Allow">Is autoswitching enabled</param>
        private void PlayNextTrack(bool Allow)
        {
            // Check if auto-play of the next song is enabled
            if (!Allow) { return; }

            // Stop the interface update timer
            durationTimer.Stop();

            // Reset the duration slider
            DurationSlider.Value = 0;

            // Initialize a variable simulating the next index
            int nextIndex = SongList.SelectedIndex + 1;

            // Exception handling
            if (nextIndex > Config.SongPaths.Count || nextIndex > SongList.Items.Count || nextIndex == -1) { return; }

            // Initialize a new song
            InitSong(nextIndex);

            // Update the maximum length of the slider
            DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;

            // Start playing the song
            PlaySong();

            // Update the label with the song name
            SongNameLabel.Content = Player.CurrentSongName;

            // Increase the index in the song list
            SongList.SelectedIndex += 1;

            // Start the interface update timer
            durationTimer.Start();
        }

        /// <summary>
        /// Event handler for the drag delta of the duration slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Update the current player time
            Player.CurrentSong.CurrentTime = TimeSpan.FromSeconds(DurationSlider.Value);
        }

        // Event handler for the duration slider value change
        private void DurationBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Check for the end of the song to avoid starting a new one while the old one is playing
            if (DurationSlider.Value == DurationSlider.Maximum)
            {
                // Automatically play the next song
                PlayNextTrack(Player.IsAutoSwitching);
            }
        }

        /// <summary>
        /// Method to initialize a song at the specified index
        /// </summary>
        /// <param name="songIndex">Index of song to init</param>
        public void InitSong(int songIndex)
        {
            // Check if the song at the specified path exists to avoid NullReferenceException
            if (!FileManager.ValidatePath(Config.SongPaths[songIndex])) return;

            // Stop playback
            Player.AudioPlayer.Stop();

            // Redefine the song in the player model
            Player.CurrentSong = new AudioFileReader(Config.SongPaths[songIndex]);

            // Initialize the player in the audio player
            Player.AudioPlayer.Init(Player.CurrentSong);

            // Clear the song name from the extension and write it to the player model
            Player.CurrentSongName = FileManager.NameFilter(Config.SongNames[songIndex]);
        }

        /// <summary>
        /// Static method for playing a song
        /// </summary>
        public static void PlaySong()
        {
            // Stop playing the current song
            Player.AudioPlayer.Stop();

            // Check if the song is not null to avoid NullReferenceException
            if (Player.CurrentSong == null) { return; }

            // Update the player status
            Player.CurrentPlayerState = PlayerStates.InProgress;

            // Start playing the song
            Player.AudioPlayer.Play();
        }

        /// <summary>
        /// Selection changed event handler for updating the selected song in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the song index is within the song array boundaries
            if (SongList.SelectedIndex == -1 || SongList.SelectedIndex > SongList.Items.Count || !isPlaylistChoosed) return;

            // Confirm that a song is selected
            isSongChoosed = true;
        }

        /// <summary>
        /// Click handler for the playlist shuffle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShuffeButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if a playlist is selected to avoid an exception
            if (!isPlaylistChoosed) { return; }

            // Shuffle the playlist
            PlayerLogic.ShufflePlaylist(PlaylistsList.SelectedIndex);

            // Update the song list
            UpdateSongList();
        }

        /// <summary>
        /// Drag enter event handler for updating the visibility of the hint label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            // Update the visibility of the hint label
            TipLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Drag leave event handler for updating the visibility of the hint label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            // Update the visibility of the hint label
            TipLabel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handler for changing the volume slider value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void audioBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                // Save the current volume
                Player.CurrentSongVolume = (float)(VolumeSlider.Value / 100);

                // Assign the new volume value to the player
                Player.AudioPlayer.Volume = Player.CurrentSongVolume;

                if (VolumeLabel != null)
                {
                    // Update the label containing the player volume
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(Player.CurrentSongVolume * 100))}%";
                }
            }
            catch { }
        }

        /// <summary>
        /// Handler for window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save the configuration
            ConfigManager.SaveConfig(PlaylistsList.SelectedIndex, SongList.SelectedIndex, (int)VolumeSlider.Value);
        }

        /// <summary>
        /// Method to set an image as the window background
        /// </summary>
        /// <param name="path"></param>
        private void SetWindowBackground(string path)
        {
            // Create ImageBrush
            ImageBrush imageBrush = new ImageBrush();

            // Create a BitmapImage object to load the image
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));

            // Set the image in ImageBrush
            imageBrush.ImageSource = bitmapImage;

            imageBrush.Stretch = Stretch.UniformToFill;

            // Set ImageBrush as the window background
            this.Background = imageBrush;
        }

        /// <summary>
        /// Click handler for the settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            var result = settingsWindow.ShowDialog();

            if (result == true)
            {
                // Set the window background if the image path is specified
                if (!string.IsNullOrEmpty(Config.BackgroundImagePath) && Config.BackgroundImagePath != "none")
                {
                    SetWindowBackground(Config.BackgroundImagePath);
                }
                if (Config.BackgroundImagePath == "none")
                {
                    SetWindowBackground(Path.Combine(Config.DefaultPath, "Icons", $"{Config.Theme}Background.png"));
                }
                UpdateUi();
                UpdatePlaylistList();
                PlaylistsList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Double-click handler for the song list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if the index is within the array bounds
            if (SongList.SelectedIndex >= 0 && SongList.SelectedIndex < SongList.Items.Count)
            {
                // Initialize and play the song
                InitSong(SongList.SelectedIndex);

                // Update the maximum length of the slider
                DurationSlider.Maximum = (int)Player.CurrentSong.TotalTime.TotalSeconds;
                PlaySong();
            }
        }

        /// <summary>
        /// Method to update the song list
        /// </summary>
        private void UpdateSongList()
        {
            SongList.ItemsSource = PlayerLogic.GetAllSongs(PlaylistsList.SelectedIndex);
        }

        /// <summary>
        /// Method to update the playlist list
        /// </summary>
        private void UpdatePlaylistList()
        {
            PlaylistsList.ItemsSource = FileManager.GetPlaylists();
        }

        /// <summary>
        /// Update the interface
        /// </summary>
        private void UpdateUi()
        { 
            // Update button images based on the configured icons
            PlayButton.Source = Config.IconsMap[1].ImageSource;
            PauseButton.Source = Config.IconsMap[2].ImageSource;
            PlayNextSongButton.Source = Config.IconsMap[3].ImageSource;
            PlayPreviosButton.Source = Config.IconsMap[4].ImageSource;
            DeletePlaylistButton.Source = Config.IconsMap[5].ImageSource;
            DeleteSongButton.Source = Config.IconsMap[5].ImageSource;
            ProgramInfoButton.Source = Config.IconsMap[7].ImageSource;
            AddPlaylistButton.Source = Config.IconsMap[8].ImageSource;
            AddSongButton.Source = Config.IconsMap[8].ImageSource;
            MuteButton.Source = Config.IconsMap[9].ImageSource;
            MoveDownButton.Source = Config.IconsMap[11].ImageSource;
            MoveUpButton.Source = Config.IconsMap[10].ImageSource;
            SettingsButton.Source = Config.IconsMap[12].ImageSource;

            // Set the foreground color for various UI elements based on the configured theme color
            var color = System.Drawing.Color.FromName(Config.Theme);
            PlaylistsList.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongList.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            PlaylistLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            ShuffeButton.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            VolumeLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            DurationLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            SongNameLabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }
    }
}