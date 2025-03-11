using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using Musical_Player.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Musical_Player.Contollers
{
    public sealed class PlaylistController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly Lazy<PlaylistController> _instance = new Lazy<PlaylistController>(() => new PlaylistController());
        public static PlaylistController Instance => _instance.Value;

        private PlaylistController() { }

        private PlaylistModel _currentPlaylist;

        public PlaylistModel CurrentPlaylist 
        {  
            get { return _currentPlaylist; }
            set
            {
                _currentPlaylist = value;
                OnPropertyChanged(nameof(CurrentPlaylist));
            }
        
        }

        /// <summary>
        /// Void to bind the propert which will call event on change 
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static List<PlaylistModel> _playlists = new List<PlaylistModel>();

        /// <summary>
        /// Creates a new playlist with the specified user name
        /// </summary>
        public void CreatePlaylist()
        {
            // Open a dialog to get the playlist name from the user
            var nameDialog = new NameDialog();
            bool? dialogResult = nameDialog.ShowDialog();
            // If the user confirms the dialog, proceed to create the playlist
            if (dialogResult == true)
            {

                // Use the user-entered name if available, otherwise use the default name
                var playlistName = nameDialog.InputTextbox.Text.Trim();

                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));
                if (xDocument.Root.Elements("playlist").Any(element => element.Attribute("name").Value == playlistName))
                {
                    MessageBox.Show("Playlist with the same name is already exists!");
                    return;
                }
                XElement newPlaylistElement = new XElement("playlist", new XAttribute("name", playlistName));
                xDocument.Root.Add(newPlaylistElement);
                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
            }
        }

        /// <summary>
        /// Adds a new song to the playlist
        /// </summary>
        public void AddSongToPlaylist()
        {
            // Open a dialog to choose songs
            OpenFileDialog opnFileDlg = new OpenFileDialog();
            opnFileDlg.Filter = "(mp3),(wav),(ogg)|*.mp3;*.wav;*.ogg;";
            opnFileDlg.Multiselect = true;

            // If files are selected, add their paths to the playlist file
            if (opnFileDlg.ShowDialog() == DialogResult.OK)
            {
                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

                XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == CurrentPlaylist.Name);

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

        /// <summary>
        /// Adds a song to the playlist from a file. Used for Drag&Drop funtion.
        /// </summary>
        /// <param name="path">Path to the song</param>
        public void AddSongToPlaylist(string path)
        {
            // Check the path and add the song to the playlist
            if (!FileManager.ValidatePath(path)) return;

            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == CurrentPlaylist.Name);

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
        /// Deletes a song from the playlist
        /// </summary>
        /// <param name="playlistName">Playlist name</param>
        /// <param name="songIndex">Song name</param>
        public void DeleteSong(int songIndex)
        {
            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == CurrentPlaylist.Name);

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
        /// Deletes a current playlist
        /// </summary>
        public void DeletePlaylist()
        {
            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == CurrentPlaylist.Name);

            playlistElement.Remove();

            xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            CurrentPlaylist = null;
        }

        /// <summary>
        /// Gets the names of playlists
        /// </summary>
        /// <returns>List of playlists</returns>
        public void LoadPlaylists()
        {
            bool erroredPath = false;
            List<PlaylistModel> playlists = new List<PlaylistModel>();

            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            foreach (var playlistElement in xDocument.Root.Elements("playlist"))
            {
                PlaylistModel loadingPlaylist = new PlaylistModel()
                {
                    Name = playlistElement.Attribute("name").Value,
                    Songs = playlistElement.Elements("song")
                    .Select(songElement => new SongModel
                    {
                        Id = int.Parse(songElement.Attribute("id").Value),
                        Path = songElement.Attribute("path").Value,
                        Name = songElement.Attribute("name").Value
                    }).ToList()
                };

                playlists.Add(loadingPlaylist);
            }

            foreach (var playlist in playlists)
            {
                foreach (var song in playlist.Songs)
                {
                    if (!FileManager.ValidatePath(song.Path))
                    {
                        DeleteSong(song.Id);
                        erroredPath = true;
                    } 
                }
            }

            _playlists = playlists;

            if (erroredPath) LoadPlaylists();
        }

        /// <summary>
        /// Moves a song up or down in the playlist
        /// </summary>
        /// <param name="songIndex">Song index in the list</param>
        /// <param name="direction">Move direction (down = 0, up = 1)</param>
        public void MoveSongIndex(int songIndex, Enums.MoveDirections direction)
        {
            try
            {
                // Calculate the new index based on the specified direction
                int newIndex = direction == Enums.MoveDirections.Down ? songIndex + 1 : songIndex - 1;
                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

                XElement playlistElement = xDocument.Root.Elements("playlist").FirstOrDefault(element => element.Attribute("name").Value == CurrentPlaylist.Name);

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
        /// Loading playlist with entered name
        /// </summary>
        /// <param name="header">Name of the playlist to be loaded</param>
        public void SelectPlaylist(string header) => CurrentPlaylist = _playlists.FirstOrDefault(playlist => playlist.Name == header);

        public List<string> GetPlaylistsHeaders() => _playlists.Select(playlist => playlist.Name).ToList();

        /// <summary>
        /// Renames the playlist
        /// </summary>
        /// <param name="playlistName"></param>
        public void RenamePlaylist(string playlistName)
        {
            // Open a dialog to get the new playlist name from the user
            var nameDialog = new NameDialog();
            bool? dialogResult = nameDialog.ShowDialog();
            // If the user confirms the dialog, proceed to create the playlist
            if (dialogResult == true)
            {

                // Use the user-entered name if available, otherwise use the default name
                var newPlaylistName = nameDialog.InputTextbox.Text.Trim();

                XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));
                if (xDocument.Root.Elements("playlist").Any(element => element.Attribute("name").Value == newPlaylistName))
                {
                    MessageBox.Show("Playlist with the same name is already exists!");
                    return;
                }
                XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == playlistName);

                playlistElement.Attribute("name").Value = newPlaylistName;

                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
            }
        }
    }
}
