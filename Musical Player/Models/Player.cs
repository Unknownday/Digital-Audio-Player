using Musical_Player.Global;
using NAudio.Wave;

namespace Musical_Player.Models
{
    /// <summary>
    /// Audio player model
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Instance of the audio player from the NAudio library
        /// </summary>
        public static WaveOutEvent AudioPlayer { get; set; } = new WaveOutEvent();

        /// <summary>
        /// Instance of the player from the NAudio library
        /// </summary>
        public static AudioFileReader CurrentSong { get; set; } = null;

        /// <summary>
        /// Storage for the previous volume
        /// </summary>
        public static float AudioValueBackup { get; set; } = 0.0F;

        /// <summary>
        /// Current volume
        /// </summary>
        public static float CurrentSongVolume { get; set; } = 0.5F;

        /// <summary>
        /// Name of the current song
        /// </summary>
        public static string CurrentSongName { get; set; } = "No song selected";

        /// <summary>
        /// Is the sound muted
        /// </summary>
        public static bool IsMuted { get; set; } = false;

        /// <summary>
        /// Is auto-switching of songs enabled
        /// </summary>
        public static bool IsAutoSwitching { get; set; } = true;

        /// <summary>
        /// Current player state
        /// </summary>
        public static Enums.PlayerStates CurrentPlayerState { get; set; } = Enums.PlayerStates.Idle;
    }
}
