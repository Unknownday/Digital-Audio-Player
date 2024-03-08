using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
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

                // Use the user-entered name if available, otherwise use the default name
                var playlistName = nameDialog.InputTextbox.Text;

                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));
                if (xDocument.Root.Elements("playlist").Any(element => element.Attribute("name").Value == playlistName))
                {
                    MessageBox.Show("Playlist with the same name is already exists!");
                }
                XElement newPlaylistElement = new XElement("playlist", new XAttribute("name", playlistName));
                xDocument.Root.Add(newPlaylistElement);
                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
            }
        }

        /// <summary>
        /// Adds a new song to the playlist
        /// </summary>
        /// <param name="playlistName">Playlist name</param>
        public static void AddSongToPlaylist(string playlistName)
        {
            // Open a dialog to choose songs
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(mp3),(wav),(ogg)|*.mp3;*.wav;*.ogg;";
            opnFileDlg.Multiselect = true;

            // If files are selected, add their paths to the playlist file
            if (opnFileDlg.ShowDialog() == true)
            {
                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

                XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == playlistName);

                foreach (string filePath in opnFileDlg.FileNames)
                {
                    SongModel newSong = new SongModel()
                    {
                        Name = Path.GetFileName(filePath),
                        Path = filePath,
                        Id = playlistElement.Elements().Count()
                    };

                    if (!playlistElement.Elements("song").Any(element => element.Attribute("id").Value == newSong.Id.ToString()))
                    {
                        XElement newSongElement = new XElement("song",
                        new XAttribute("id", newSong.Id),
                        new XAttribute("name", newSong.Name),
                        new XAttribute("path", newSong.Path));

                        playlistElement.Add(newSongElement);
                    }
                }
                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
            }
        }

        //// <summary>
        /// Deletes a song from the playlist
        /// </summary>
        /// <param name="playlistName">Playlist name</param>
        /// <param name="songIndex">Song name</param>
        public static void DeleteSong(string playlistName, int songIndex)
        {
            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == playlistName);

            if (playlistElement != null)
            {
                XElement songElement = playlistElement.Elements("song")
                    .FirstOrDefault(element => element.Attribute("id").Value == songIndex.ToString());

                songElement?.Remove();

                int newIndex = 0;
                foreach (XElement remainingSong in playlistElement.Elements("song"))
                {
                    remainingSong.Attribute("id").Value = newIndex.ToString();
                    newIndex++;
                }
            }

            xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
        }

        /// <summary>
        /// Deletes a playlist
        /// </summary>
        /// <param name="playlistName">Playlist index</param>
        public static void DeletePlaylist(string playlistName)
        {
            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == playlistName);

            playlistElement.Remove();

            xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
        }

        /// <summary>
        /// Moves a song up or down in the playlist
        /// </summary>
        /// <param name="songIndex">Song index in the list</param>
        /// <param name="playlistName">Playlist index</param>
        /// <param name="direction">Move direction (down = 0, up = 1)</param>
        public static void MoveSongInQueue(int songIndex, string playlistName, Enums.MoveDirections direction)
        {
            try
            {
                // Calculate the new index based on the specified direction
                int newIndex = direction == Enums.MoveDirections.Down ? songIndex + 1 : songIndex - 1;
                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

                XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == playlistName);

                // Check if the new index is within valid bounds
                if (newIndex >= 0 && newIndex < playlistElement.Elements("song").Count())
                {
                    if (playlistElement != null)
                    {
                        XElement songElement = playlistElement.Elements("song").FirstOrDefault(element => element.Attribute("id").Value == songIndex.ToString());

                        if (songElement != null)
                        {

                            XElement nextSongElement = playlistElement.Elements("song").FirstOrDefault(element => element.Attribute("id").Value == newIndex.ToString());

                            if (nextSongElement != null)
                            {
                                songElement.Attribute("id").Value = newIndex.ToString();
                                nextSongElement.Attribute("id").Value = songIndex.ToString();

                                playlistElement.ReplaceNodes(playlistElement.Elements("song").OrderBy(element => int.Parse(element.Attribute("id").Value)));

                                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
                            }
                        }
                    }
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
            bool erroredPath = false;
            Dictionary<string, List<SongModel>> playlists = new Dictionary<string, List<SongModel>>();

            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            foreach (var playlistElement in xDocument.Root.Elements("playlist"))
            {
                string playlistName = playlistElement.Attribute("name").Value;
                List<SongModel> songs = playlistElement.Elements("song")
                    .Select(songElement => new SongModel
                    {
                        Id = int.Parse(songElement.Attribute("id").Value),
                        Path = songElement.Attribute("path").Value,
                        Name = songElement.Attribute("name").Value
                    })
                    .ToList();

                playlists.Add(playlistName, songs);
            }

            foreach (var key in playlists.Keys)
            {
                for(int i = 0; i < playlists[key].Count(); i++)
                {
                    if (!ValidatePath(playlists[key][i].Path))
                    {
                        DeleteSong(key, i);
                        erroredPath = true;
                    }
                }
            }

            if (erroredPath)
            {
                return GetPlaylists();
            }
            Config.PlaylistToSongListMap = playlists;
            return playlists.Keys.ToList();
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

            if (!File.Exists(Path.Combine(Config.DefaultPath, "Playlists.xml")))
            {
                XElement playlistsElement = new XElement("playlists");
                XDocument xDocument = new XDocument(playlistsElement);
                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
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

            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == playlistName);

            SongModel newSong = new SongModel()
            {
                Name = Path.GetFileName(path),
                Path = path,
                Id = playlistElement.Elements().Count()
            };

            if (!playlistElement.Elements("song").Any(element => element.Attribute("id").Value == newSong.Id.ToString()))
            {
                XElement newSongElement = new XElement("song",
                new XAttribute("id", newSong.Id),
                new XAttribute("name", newSong.Name),
                new XAttribute("path", newSong.Path));

                playlistElement.Add(newSongElement);
            }

            xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
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
