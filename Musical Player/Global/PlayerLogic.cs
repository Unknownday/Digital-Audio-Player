using Musical_Player.Files_management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace Musical_Player.Global
{
    /// <summary>
    /// Main logic for the player
    /// </summary>
    public static class PlayerLogic
    {
        /// <summary>
        /// Returns formatted time for the time label
        /// </summary>
        /// <param name="currMinutes">Current minute of the playing song</param>
        /// <param name="currSeconds">Current second of the playing song</param>
        /// <param name="totMinutes">Total minutes of the song</param>
        /// <param name="totSeconds">Total seconds of the song</param>
        /// <returns>Formatted string with song time ready for use</returns>
        public static string GetDurationLabel(int currMinutes, int currSeconds, int totMinutes, int totSeconds)
        {
            // Format current and total duration of the song as "mm:ss"
            string currentDuration = $"{currMinutes:D2}:{currSeconds:D2}";
            string totalDuration = $"{totMinutes:D2}:{totSeconds:D2}";

            // Return the string with formatted time in the format "current | total"
            return $"{currentDuration} | {totalDuration}";
        }

        /// <summary>
        /// Gets the names of all songs in the playlist
        /// </summary>
        /// <param name="playlistName">Name of the playlist</param>
        /// <returns>List of all song names</returns>
        public static List<string> GetAllSongs(string playlistName)
        {
            var songList = Config.PlaylistToSongListMap[playlistName];

            // Clear the lists in Config and add existing paths and song names
            Config.SongPaths.Clear();
            Config.SongNames.Clear();
            Config.SongPaths.AddRange(songList.Select(x => x.Path).Where(x => FileManager.ValidatePath(x)));
            Config.SongNames.AddRange(songList.Select(x => FileManager.NameFilter($"{x.Id+1}. {x.Name}")));

            return Config.SongNames;
        }

        /// <summary>
        /// Shuffles the songs in the playlist
        /// </summary>
        /// <param name="playlistName">Name of the playlist</param>
        public static void ShufflePlaylist(string playlistName)
        {
            // Load the playlist from the XML file
            XDocument xDocument = XDocument.Load(Path.Combine(Config.DefaultPath, "Playlists.xml"));

            XElement playlistElement = xDocument.Root.Elements("playlist")
                .FirstOrDefault(element => element.Attribute("name").Value == playlistName);

            if (playlistElement != null)
            {
                // Get the list of song elements in the playlist
                List<XElement> songElements = playlistElement.Elements("song").ToList();

                // Shuffle the song elements randomly
                Shuffle(songElements);

                // Clear existing songs in the playlist
                playlistElement.Elements("song").Remove();

                // Add the shuffled songs back to the playlist
                playlistElement.Add(songElements);

                int newIndex = 0;
                foreach (XElement remainingSong in playlistElement.Elements("song"))
                {
                    remainingSong.Attribute("id").Value = newIndex.ToString();
                    newIndex++;
                }

                // Save the modified XML document
                xDocument.Save(Path.Combine(Config.DefaultPath, "Playlists.xml"));
            }
        }

        /// <summary>
        /// Shuffles a list of XElement elements
        /// </summary>
        /// <param name="list">List of XElement elements to shuffle</param>
        public static void Shuffle(List<XElement> list)
        {
            // Initialize the random number generator
            Random random = new Random();

            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                // Generate a random index
                int j = random.Next(0, i + 1);

                // Swap the values of list[i] and list[j]
                XElement temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
