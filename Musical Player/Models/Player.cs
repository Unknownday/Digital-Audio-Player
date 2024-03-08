using Musical_Player.Global;
using NAudio.Wave;

namespace Musical_Player.Models
{
    /// <summary>
    /// Модель аудиоплеера
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Экземпляр аудиоплеера из библиотеки NAudio
        /// </summary>
        public static WaveOutEvent AudioPlayer { get; set; } = new WaveOutEvent();

        /// <summary>
        /// Экземпляр проигрывателя из библиотеки NAudio
        /// </summary>
        public static AudioFileReader CurrentSong { get; set; } = null;

        /// <summary>
        /// Хранилище предыдущей громкости
        /// </summary>
        public static float AudioValueBackup { get; set; } = 0.0F;

        /// <summary>
        /// Текущая громкость
        /// </summary>
        public static float CurrentSongVolume { get; set; } = 0.5F;

        /// <summary>
        /// Название текущей песни
        /// </summary>
        public static string CurrentSongName { get; set; } = "Песня не выбрана";

        /// <summary>
        /// Выключен ли звук
        /// </summary>
        public static bool IsMuted { get; set; } = false;

        /// <summary>
        /// Включено-ли автопереключение песни
        /// </summary>
        public static bool IsAutoSwitching { get; set; } = true;

        /// <summary>
        /// Текущее состояние плеера
        /// </summary>
        public static Enums.PlayerStates CurrentPlayerState { get; set; } = Enums.PlayerStates.Idle;
    }
}
