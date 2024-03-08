using Musical_Player.Files_management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="playlistIndex">Index of the playlist</param>
        /// <returns>List of all song names</returns>
        public static List<string> GetAllSongs(int playlistIndex)
        {
            // Read all lines from the playlist file
            var songPaths = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Check the existence of each song in the list, remove those that are missing
            for (int i = 0; i < songPaths.Length; i++)
            {
                if (!FileManager.ValidatePath(songPaths[i]))
                {
                    FileManager.DeleteSong(playlistIndex, i);
                }
            }

            // Clear the lists in Config and add existing paths and song names
            Config.SongNames.Clear();
            Config.SongPaths.Clear();
            Config.SongPaths.AddRange(songPaths);
            Config.SongNames.AddRange(songPaths.Select(songPath => Path.GetFileName(songPath)));

            // Return the formatted list of song names
            return Config.SongPaths.Select((path, index) => FileManager.NameFilter($"{index + 1}. {Path.GetFileName(path)}")).ToList();
        }

        /// <summary>
        /// Shuffles the songs in the playlist
        /// </summary>
        /// <param name="playlistIndex">Index of the playlist</param>
        public static void ShufflePlaylist(int playlistIndex)
        {
            // Read all lines from the playlist file
            var currentPlaylist = File.ReadAllLines(Config.PlaylistPaths[playlistIndex]);

            // Call the Shuffle method for random shuffling of elements
            Shuffle(currentPlaylist);

            // Write the shuffled playlist back to the file
            File.WriteAllLines(Config.PlaylistPaths[playlistIndex], currentPlaylist);
        }

        /// <summary>
        /// Randomly shuffles the indices in the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Array to shuffle</param>
        public static void Shuffle<T>(T[] array)
        {
            // Initialize the random number generator
            Random random = new Random();

            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                // Generate a random index
                int j = random.Next(0, i + 1);

                // Swap the values of array[i] and array[j]
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
