using System.Collections.Generic;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Musical_Player.Global
{
    public static class Config
    {
        /// <summary>
        /// Список путей к песням
        /// </summary>
        public static List<string> SongPaths { get; set; } = new List<string>();

        /// <summary>
        /// Список путей к плейлистам
        /// </summary>
        public static List<string> PlaylistPaths { get; set; } = new List<string>();

        /// <summary>
        /// Список названий песен
        /// </summary>
        public static List<string> SongNames { get; set; } = new List<string>();

        public static Dictionary<int, ImageBrush> IconsMap { get; set; }

        public static Dictionary<int, StackPanel> tagToStackPanelMap { get; set; } = new Dictionary<int, StackPanel>();

        /// <summary>
        /// Стандартный путь к директории, в которой хранятся плейлисты
        /// </summary>
        public static string DefaultPath { get; set; } = @"C:\\Digital audio player\";

        /// <summary>
        /// Путь к изображению фона
        /// </summary>
        public static string BackgroundImagePath { get; set; } = "none";

        /// <summary>
        /// Индекс последнего выбранного плейлиста
        /// </summary>
        public static int LastPlaylist { get; set; } = -1;

        /// <summary>
        /// Индекс последней выбранной песни
        /// </summary>
        public static int LastSong { get; set; } = -1;

        public static int LastVolume { get; set; } = 50;

        public static string Theme { get; set; } = "Black";

        public const string VERSION = "1.0.0.0";

    }
}
