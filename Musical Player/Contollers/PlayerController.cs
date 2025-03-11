using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Musical_Player.Global.Enums;

namespace Musical_Player.Contollers
{
    public static class PlayerController
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

        public static void PlayRecording(int songIndex)
        {
            // Check if a playlist and a song are selected
            if (PlaylistController.Instance.CurrentPlaylist == null || PlaylistController.Instance.CurrentPlaylist.Songs.Count <= 0) { return; }
           
            // Depending on the current player state, perform the corresponding actions
            switch (PlayerModel.Instance.CurrentPlayerState)
            {
                case PlayerStates.Idle:
                    PlayerModel.Instance.CurrentSong = new AudioFileReader(PlaylistController.Instance.CurrentPlaylist.Songs[songIndex].Path);

                    PlayerModel.Instance.AudioPlayer.Play();
                break;

                case PlayerStates.InProgress:
                    PlayerModel.Instance.AudioPlayer.Stop();

                    PlayerModel.Instance.CurrentSong = new AudioFileReader(PlaylistController.Instance.CurrentPlaylist.Songs[songIndex].Path);

                    PlayerModel.Instance.AudioPlayer.Play();
                break;

                case PlayerStates.OnPause:
                    PlayerModel.Instance.CurrentPlayerState = PlayerStates.InProgress;

                    PlayerModel.Instance.AudioPlayer.Play();
                break;
            }
        }

        public static void StopRecording()
        {
            PlayerModel.Instance.AudioPlayer.Stop();

            PlayerModel.Instance.CurrentPlayerState = PlayerStates.Idle;
        }

        public static void PauseRecoding()
        {
            PlayerModel.Instance.AudioPlayer.Pause();

            PlayerModel.Instance.CurrentPlayerState = PlayerStates.OnPause;
        }
    }
}
