using Microsoft.Win32;
using Musical_Player.Config_management;
using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.LoadingLogic;
using Musical_Player.Models;
using Musical_Player.ViewModels;
using Musical_Player.Views;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        /// Variable to track if enabled repeating song
        /// </summary>
        public bool isRepeating = false;

        /// <summary>
        /// Constructor for the MainWindow class
        /// </summary>
        public MainWindow()
        {
            // Preparing programm to work
            StartupProgram();

            // Initialize program components
            InitializeComponent();
        }

        /// <summary>
        /// This one is used only for make code "clean"
        /// </summary>
        private void StartupProgram()
        {
            //Creating the temple keys dictionary
            Dictionary<string, string> keys = new Dictionary<string, string>();

            //Reading the part of config which storaged in registry
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\DigitalAudioPlayer\PlayerConfig"))
            {
                keys.Add("DefaultPath", key.GetValue("DefaultPath") as string);
                keys.Add("Background", key.GetValue("Background") as string);
            }

            //Updating the default programm path
            Config.DefaultPath = keys["DefaultPath"] == null ? "C:\\Digital Audio Player\\" : keys["DefaultPath"];

            //Updating the default programm background image path
            Config.BackgroundImagePath = keys["Background"] == "none" ? "none" : keys["Background"];

            //Creating icon dictionary from image
            //SetupLogic.CreateIconsBitmap();

            //Updating graphical interface
            //UpdateUi();

            LoadLanguageConfig();

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

            // Create a timer for updating the interface
            durationTimer = CreateTimer(new EventHandler(UpdateDuration));

            // Start the timer
            durationTimer.Start();

            // Set the value for the volume slider
            //VolumeSlider.Value = Config.LastVolume;

            //Update the playlist list
            UpdatePlaylistList();

            // Restore the previous playlist
            //PlaylistsList.SelectedIndex = Config.LastPlaylist;

            //Awaiting until songs will be loaded;
            // while (SongList.Items.Count == 0 && SongList.Items == null) { }

            // Restore the previous song
            //SongList.SelectedIndex = Config.LastSong;

            //Set the value for the volume label
            // VolumeLabel.Content = $"{Config.LastVolume}%";

            //Restore last song volume
            // PlayerModel.CurrentSongVolume = (float)(Config.LastVolume / 100);

            ThemeManager themeManager = ThemeManager.Instance;

            MainWindowViewModel mainWindowViewModel = MainWindowViewModel.Instance;

            this.DataContext = mainWindowViewModel;
        }

        private void LoadLanguageConfig()
        {
            LanguageModel model = new LanguageModel();

            model.AutoSwitchBtnText = "Enable/Disable Auto song switching";
            model.CloseBtnText = "Close information window";
            model.Label1Text = "Player created using:";
            model.Label2Text = "NAudio library";
            model.SongsTagText = "Your Songs:";
            model.PlaylistsTagText = "Your Playlists:";
            model.ChangeBgBtnText = "Choose background image";
            model.ChangeDirectoryBtnText = "Change player default directory";
            model.BuildVerText = $"Build: {Config.VERSION}";
            model.ShuffleBtnText = "Shuffle songs";
            model.SubmitBtnText = "Confirm";
            model.TextBoxTag = "Input new playlist name";
            model.ThemeToggler = "Enable custom theme";
            model.SwitchThemeTag = "Enable light theme";
            model.RepeatIndicator = "REPEAT";
            model.SongNameText = "Select song first";

            Config.LanguageModel = model;
        }

        /// <summary>
        /// Function is used for create timer
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private Timer CreateTimer(EventHandler handler)
        {
            var timer = new Timer();

            // Set the timer interval in milliseconds
            timer.Interval = 1;

            // Set timer to be enabled
            timer.Enabled = true;

            // Set timer tag
            timer.Tag = "Duration Timer";

            // Attach event handler for timer updates
            timer.Tick += new EventHandler(handler);

            // Return new timer;
            return timer;
        } 

        /// <summary>
        /// Event handler for the "Drop" event when dragging files into the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            // Hide the tooltip
            //TipLabel.Visibility = Visibility.Hidden;

            // Check if a playlist is selected
            if (!isPlaylistChoosed) { MessageBox.Show("Select a playlist first!", "Musical player"); return; }

            // Get the list of files dragged into the window
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check if each file is a supported audio format
            if (files.Any(file => !FileManager.IsSupportedAudioFormat(file))) { return; }

            // Add each supported audio file to the selected playlist
            //files.Where(FileManager.IsSupportedAudioFormat).ToList().ForEach(file => FileManager.AddSong(file, PlaylistsList.SelectedItem.ToString()));
            
            var currentIndex = PlaylistsList.SelectedIndex;
            UpdatePlaylistList();
            PlaylistsList.SelectedIndex = currentIndex;

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
                var tempSoundValue = PlayerModel.CurrentSongVolume;

                // Mute the sound
                PlayerModel.CurrentSongVolume = PlayerModel.AudioValueBackup;

                // Save the current volume for restoration
                PlayerModel.AudioValueBackup = tempSoundValue;

                // Update the player state
                PlayerModel.IsMuted = !PlayerModel.IsMuted;

                // Set the value for the volume slider
                VolumeSlider.Value = PlayerModel.CurrentSongVolume * 100;

                // Set the value for the volume label
                VolumeLabel.Content = $"{Convert.ToInt32(Math.Abs(PlayerModel.CurrentSongVolume * 100))}%";

                // Set the new volume value for the player
                PlayerModel.AudioPlayer.Volume = PlayerModel.CurrentSongVolume;
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
               // FileManager.CreatePlaylist();

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
                //FileManager.DeletePlaylist(PlaylistsList.SelectedItem.ToString());

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
                //FileManager.MoveSongInQueue(currentIndex, PlaylistsList.SelectedItem.ToString(), Enums.MoveDirections.Up);

                // Update the playlist list
                UpdatePlaylistList();
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
                //FileManager.MoveSongInQueue(currentIndex, PlaylistsList.SelectedItem.ToString(), Enums.MoveDirections.Down);

                // Update the playlist list
                UpdatePlaylistList();
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
               // FileManager.AddSongToPlaylist(PlaylistsList.SelectedItem.ToString());

                // Update the song and playlist list
                UpdatePlaylistList();
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
                // Delete the selected song from the playlist
               // FileManager.DeleteSong(PlaylistsList.SelectedItem.ToString(), SongList.SelectedIndex);

                // Update the playlist list
                UpdatePlaylistList();

                // Update the song list
                UpdateSongList();
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
                DurationSlider.Maximum = (int)PlayerModel.CurrentSong.TotalTime.TotalSeconds;

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
                PlayerModel.CurrentPlayerState = PlayerStates.OnPause;

                // Pause playback
                PlayerModel.AudioPlayer.Pause();
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
                switch (PlayerModel.CurrentPlayerState)
                {
                    case PlayerStates.Idle:
                    case PlayerStates.InProgress:
                        // Initialize the song
                        InitSong(SongList.SelectedIndex);

                        // Set the maximum value of the duration slider equal to the song duration
                        DurationSlider.Maximum = (int)PlayerModel.CurrentSong.TotalTime.TotalSeconds;

                        // Play the song
                        PlaySong();

                        // Update the label with the song name
                        SongNameLabel.Content = PlayerModel.CurrentSongName;

                        // Start the interface update timer
                        durationTimer.Start();
                        break;

                    case PlayerStates.OnPause:
                        // Update the player state
                        PlayerModel.CurrentPlayerState = PlayerStates.InProgress;

                        // Resume playback of the song
                        PlayerModel.AudioPlayer.Play();

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
                DurationSlider.Maximum = (int)PlayerModel.CurrentSong.TotalTime.TotalSeconds;

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
                if (PlayerModel.CurrentSong != null)
                {
                    // Update the label with the duration
                    DurationLabel.Content = PlayerLogic.GetDurationLabel(PlayerModel.CurrentSong.CurrentTime.Minutes,
                            PlayerModel.CurrentSong.CurrentTime.Seconds,
                            PlayerModel.CurrentSong.TotalTime.Minutes,
                            PlayerModel.CurrentSong.TotalTime.Seconds
                        );

                    // Set the value of the duration slider
                    DurationSlider.Value = PlayerModel.CurrentSong.CurrentTime.Seconds + 60 * PlayerModel.CurrentSong.CurrentTime.Minutes;

                    // Display the current status in the window title
                    string playbackState = PlayerModel.CurrentPlayerState == PlayerStates.InProgress ? "▶" : "||";
                    Title = PlayerModel.IsAutoSwitching ? $"{playbackState} {PlayerModel.CurrentSongName} |⇌|" : $"{playbackState} {PlayerModel.CurrentSongName}";

                    // Update the label with the song name
                    SongNameLabel.Content = PlayerModel.CurrentSongName;

                    // Update the label with the volume
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(PlayerModel.CurrentSongVolume * 100))}%";
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
            DurationSlider.Maximum = (int)PlayerModel.CurrentSong.TotalTime.TotalSeconds;

            // Start playing the song
            PlaySong();

            // Update the label with the song name
            SongNameLabel.Content = PlayerModel.CurrentSongName;

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
            PlayerModel.CurrentSong.CurrentTime = TimeSpan.FromSeconds(DurationSlider.Value);
        }

        /// <summary>
        /// Event handler for the duration slider value change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DurationBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Check for the end of the song to avoid starting a new one while the old one is playing
            if (DurationSlider.Value == DurationSlider.Maximum)
            {
                if (isRepeating)
                {
                    DurationSlider.Value = 0;
                    InitSong(SongList.SelectedIndex);
                    PlaySong();
                    return;
                }

                // Automatically play the next song
                PlayNextTrack(PlayerModel.IsAutoSwitching);
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
            PlayerModel.AudioPlayer.Stop();

            // Redefine the song in the player model
            PlayerModel.CurrentSong = new AudioFileReader(Config.SongPaths[songIndex]);

            // Initialize the player in the audio player
            PlayerModel.AudioPlayer.Init(PlayerModel.CurrentSong);

            // Clear the song name from the extension and write it to the player model
            PlayerModel.CurrentSongName = FileManager.NameFilter(Config.SongNames[songIndex]);
        }

        /// <summary>
        /// Static method for playing a song
        /// </summary>
        public static void PlaySong()
        {
            // Stop playing the current song
            PlayerModel.AudioPlayer.Stop();

            // Check if the song is not null to avoid NullReferenceException
            if (PlayerModel.CurrentSong == null) { return; }

            // Update the player status
            PlayerModel.CurrentPlayerState = PlayerStates.InProgress;

            // Start playing the song
            PlayerModel.AudioPlayer.Play();
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
            PlayerLogic.ShufflePlaylist(PlaylistsList.SelectedItem.ToString());
            UpdatePlaylistList();

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
            //TipLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Drag leave event handler for updating the visibility of the hint label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            // Update the visibility of the hint label
            //TipLabel.Visibility = Visibility.Hidden;
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
                PlayerModel.CurrentSongVolume = (float)(VolumeSlider.Value / 100);

                // Assign the new volume value to the player
                PlayerModel.AudioPlayer.Volume = PlayerModel.CurrentSongVolume;

                if (VolumeLabel != null)
                {
                    // Update the label containing the player volume
                    VolumeLabel.Content = $"{(int)Math.Round(Math.Abs(PlayerModel.CurrentSongVolume * 100))}%";
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
                //UpdateUi();
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
                DurationSlider.Maximum = (int)PlayerModel.CurrentSong.TotalTime.TotalSeconds;
                PlaySong();
            }
        }

        /// <summary>
        /// Method to update the song list
        /// </summary>
        private void UpdateSongList()
        {
            int lastIndex = SongList.SelectedIndex;

            var ViewModelInstance = MainWindowViewModel.Instance;

            ViewModelInstance.UpdateSongs(PlayerLogic.GetAllSongs(PlaylistsList.SelectedItem.ToString()));

            this.DataContext = ViewModelInstance;

            SongList.SelectedIndex = lastIndex;
        }

        /// <summary>
        /// Method to update the playlist list
        /// </summary>
        private void UpdatePlaylistList()
        {
            //int lastIndex = PlaylistsList.SelectedIndex;

            //var ViewModelInstance = MainWindowViewModel.Instance;

            //ViewModelInstance.UpdatePlaylist(FileManager.GetPlaylists());

            //this.DataContext = ViewModelInstance;

            //PlaylistsList.SelectedIndex = lastIndex;
        }


        /// <summary>
        /// Click handler for repeat button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatSongButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Updating variable value
            isRepeating = !isRepeating;

            //// Updating repeating indicator visibility
            //RepeatIndicatorLabel.Visibility = isRepeating ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Click handler for rename playlis button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenamePlaylistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Renaming playlist 
            //FileManager.RenamePlaylist(PlaylistsList.SelectedItem.ToString());

            //Updating playlists
            UpdatePlaylistList();
        }
        
    }
}