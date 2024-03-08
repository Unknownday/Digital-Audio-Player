using Musical_Player.Global;
using Musical_Player.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
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
        /// Creates a new playlist with the specified user name
        /// </summary>
        public static void CreatePlaylist()
        {
            // Open a dialog to get the playlist name from the user
            var nameDialog = new NameDialog();
            bool? dialogResult = nameDialog.ShowDialog();

            // If the user confirms the dialog, proceed to create the playlist
            if (dialogResult == true)
            {
                // Default playlist name
                string playlistName = "New Playlist";

                // Use the user-entered name if available, otherwise use the default name
                playlistName = nameDialog.InputTextbox.Text == null ? "New Playlist" : nameDialog.InputTextbox.Text + ".playlist";

                // Check if a playlist with the same name already exists
                if (File.Exists(Path.Combine(Config.DefaultPath, playlistName)))
                {
                    MessageBox.Show("A playlist with this name already exists! Please choose another.", "Music Player");
                    return;
                }

                // Create an empty playlist file
                File.Create(Path.Combine(Config.DefaultPath, playlistName)).Close();
            }
        }

        /// <summary>
        /// Adds a new song to the playlist
        /// </summary>
        /// <param name="playlistIndex">Playlist index</param>
        public static void AddSongToPlaylist(int playlistIndex)
        {
            // Open a dialog to choose songs
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(mp3),(wav),(ogg)|*.mp3;*.wav;*.ogg;";
            opnFileDlg.Multiselect = true;

            // If files are selected, add their paths to the playlist file
            if (opnFileDlg.ShowDialog() == true)
            {
                foreach (string fileName in opnFileDlg.FileNames)
                {
                    File.AppendAllText(Config.PlaylistPaths[playlistIndex], fileName + "\n");
                }
            }
        }

        /// <summary>
        /// Deletes a song from the playlist
        /// </summary>
        /// <param name="playlistIndex">Playlist index</param>
        /// <param name="songIndex">Song index in the list</param>
        public static void DeleteSong(int playlistIndex, int songIndex)
        {
            // Read the current playlist
            var currentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Create a new playlist excluding the specified song
            var newPlaylist = currentPlaylist.Where((x, index) => index != songIndex).ToList();

            // Write the new playlist back to the file
            File.WriteAllLines(Config.PlaylistPaths[playlistIndex], newPlaylist);
        }

        /// <summary>
        /// Deletes a playlist
        /// </summary>
        /// <param name="playlistIndex">Playlist index</param>
        public static void DeletePlaylist(int playlistIndex)
        {
            // Delete the playlist file
            File.Delete(Config.PlaylistPaths[playlistIndex]);
        }

        /// <summary>
        /// Moves a song up or down in the playlist
        /// </summary>
        /// <param name="songIndex">Song index in the list</param>
        /// <param name="playlistIndex">Playlist index</param>
        /// <param name="direction">Move direction (down = 0, up = 1)</param>
        public static void MoveSongInQueue(int songIndex, int playlistIndex, Enums.MoveDirections direction)
        {
            try
            {
                // Read the current playlist
                var currentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

                // Calculate the new index based on the specified direction
                int newIndex = direction == Enums.MoveDirections.Down ? songIndex + 1 : songIndex - 1;

                // Check if the new index is within valid bounds
                if (newIndex >= 0 && newIndex < currentPlaylist.Length)
                {
                    // Swap the current song with the one at the new index
                    Swap(currentPlaylist, songIndex, newIndex);

                    // Write the modified playlist back to the file
                    File.WriteAllLines(Config.PlaylistPaths[playlistIndex], currentPlaylist);
                }
                else
                {
                    // Display an error message for an invalid move operation
                    Console.WriteLine("Invalid song move operation.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and display an error message
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the names of playlists
        /// </summary>
        /// <returns>List of playlists</returns>
        public static List<string> GetPlaylists()
        {
            // Clear the list of song paths in the configuration
            Config.SongPaths.Clear();

            // Get all files with the ".playlist" extension in the default directory
            IEnumerable<string> playlists = Directory.EnumerateFiles(Config.DefaultPath, "*.playlist");

            // Clear and add playlist paths to the list
            Config.PlaylistPaths.Clear();
            Config.PlaylistPaths.AddRange(playlists);

            // Extract safe file names and add them to the result and playlist paths to the configuration
            return playlists.Select(playlist => NameFilter(Path.GetFileName(playlist))).ToList();
        }

        /// <summary>
        /// Swaps elements in an array
        /// </summary>
        /// <param name="array">Array in which to swap elements</param>
        /// <param name="index1">First index to swap</param>
        /// <param name="index2">Second index to swap</param>
        private static void Swap<T>(T[] array, int index1, int index2)
        {
            // Temporary variable for swapping values
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
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
        /// Checks if the required directory exists
        /// </summary>
        public static void ValidateDefaultPath(string path)
        {
            // Create directories if they do not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(Path.Combine(path, "Icons")))
            {
                Directory.CreateDirectory(Path.Combine(path, "Icons"));
            }

            if (!File.Exists(Path.Combine(path, "Icons", "BlackIconSet.png"))
                || !File.Exists(Path.Combine(path, "Icons", "WhiteIconSet.png"))
                || !File.Exists(Path.Combine(path, "Icons", "WhiteBackground.png"))
                || !File.Exists(Path.Combine(path, "Icons", "BlackBackground.png")))
            {
                DownloadMiscFiles();
            }
        }

        /// <summary>
        /// Adds a song to the playlist from a file. Used for Drag & Drop.
        /// </summary>
        /// <param name="path">Path to the song</param>
        /// <param name="playlistName">Name of the playlist to which to add the song</param>
        public static void AddSong(string path, string playlistName)
        {
            // Check the path and add the song to the playlist
            if (!ValidatePath(path))
            {
                return;
            }

            File.AppendAllText(Path.Combine(Config.DefaultPath, playlistName + ".playlist"), path + "\n");
        }

        /// <summary>
        /// Checks if the song format is supported for Drag & Drop
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
                ValidateDefaultPath(newDefaultPath);
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
        /// Downloads miscellaneous files
        /// </summary>
        public static void DownloadMiscFiles()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Download files from URLs to the specified directory
                    client.DownloadFile("https://github.com/Unknownday/CS-Audio-Player/blob/Static-resources/BlackIconSet.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "BlackIconSet.png"));
                    client.DownloadFile("https://github.com/Unknownday/CS-Audio-Player/blob/Static-resources/WhiteIconSet.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "WhiteIconSet.png"));
                    client.DownloadFile("https://github.com/Unknownday/CS-Audio-Player/blob/Static-resources/BlackBackground.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "BlackBackground.png"));
                    client.DownloadFile("https://github.com/Unknownday/CS-Audio-Player/blob/Static-resources/WhiteBackground.png?raw=true", Path.Combine(Config.DefaultPath, "Icons", "WhiteBackground.png"));
                }
                catch { }
            }
        }
    }
}
