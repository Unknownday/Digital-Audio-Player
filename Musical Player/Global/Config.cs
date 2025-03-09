using Musical_Player.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Musical_Player.Global
{
    /// <summary>
    /// Programm configuration file.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// List of paths to songs.
        /// </summary>
        public static List<string> SongPaths { get; set; } = new List<string>();

        /// <summary>
        /// List of paths to playlists.
        /// </summary>
        public static List<string> PlaylistPaths { get; set; } = new List<string>();

        /// <summary>
        /// List of song names.
        /// </summary>
        public static List<string> SongNames { get; set; } = new List<string>();

        /// <summary>
        /// Dictionary mapping integers to ImageBrush objects.
        /// </summary>
        public static Dictionary<int, ImageBrush> IconsMap { get; set; }

        /// <summary>
        /// Dictionary mapping playlist(name) to List of the songs(models).
        /// </summary>
        public static Dictionary<string, List<SongModel>> PlaylistToSongListMap { get; set; } = new Dictionary<string, List<SongModel>>();

        /// <summary>
        /// Default path to the directory where playlists are stored.
        /// </summary>
        public static string DefaultPath { get; set; } = null;

        /// <summary>
        /// Path to the background image.
        /// </summary>
        public static string BackgroundImagePath { get; set; } = "none";

        /// <summary>
        /// Index of the last selected playlist.
        /// </summary>
        public static int LastPlaylist { get; set; } = -1;

        /// <summary>
        /// Index of the last selected song.
        /// </summary>
        public static int LastSong { get; set; } = -1;

        /// <summary>
        /// Last volume level.
        /// </summary>
        public static int LastVolume { get; set; } = 50;

        /// <summary>
        /// Theme color setting.
        /// </summary>
        public static string Theme { get; set; } = "Black";

        /// <summary>
        /// Constant representing the version number.
        /// </summary>
        public const string VERSION = "1.1.5.2";

        /// <summary>
        /// Programm elements text holder
        /// </summary>
        public static LanguageModel LanguageModel { get; set; }
    }
}
