using Musical_Player.Global;
using NAudio.Wave;
using System.ComponentModel;

namespace Musical_Player.Models
{
    /// <summary>
    /// Audio player model
    /// </summary>
    public sealed class PlayerModel : INotifyPropertyChanged
    {
        private static PlayerModel _instance;
        private static AudioFileReader _currentSong;
        private static string _currentSongName;
        public static PlayerModel Instance {
            get
            {
                if (_instance == null) 
                { 
                    _instance = new PlayerModel(); 
                } 
                return _instance;
            }
        }

        private PlayerModel() { }

        /// <summary>
        /// Instance of the audio player from the NAudio library
        /// </summary>
        public WaveOutEvent AudioPlayer { get; set; } = new WaveOutEvent();

        /// <summary>
        /// Instance of the player from the NAudio library
        /// </summary>
        public AudioFileReader CurrentSong 
        { 
            get
            {
                return _currentSong;
            }
            set 
            {
                _currentSong = value;
                OnPropertyChanged(nameof(CurrentSong));
            } 
        }

        /// <summary>
        /// Storage for the previous volume
        /// </summary>
        public float AudioValueBackup { get; set; } = 0.0F;

        /// <summary>
        /// Current volume
        /// </summary>
        public float CurrentSongVolume { get; set; } = 0.5F;

        /// <summary>
        /// Name of the current song
        /// </summary>
        public  string CurrentSongName
        {
            get
            {
                return _currentSongName;
            }
            set
            {
                _currentSongName = value;
                OnPropertyChanged(nameof(CurrentSongName));
            }
        }

        /// <summary>
        /// Is the sound muted
        /// </summary>
        public  bool IsMuted { get; set; } = false;

        /// <summary>
        /// Is auto-switching of songs enabled
        /// </summary>
        public  bool IsAutoSwitching { get; set; } = true;

        /// <summary>
        /// Current player state
        /// </summary>
        public  Enums.PlayerStates CurrentPlayerState { get; set; } = Enums.PlayerStates.Idle;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
