using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Musical_Player.ViewModels
{
    /// <summary>
    /// The viewmodel for 
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private static MainWindowViewModel instance;
        public static MainWindowViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainWindowViewModel();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event which is called when choosen propretry has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Void to bind the propert which will call event on change 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<string> _songs { get; set; }
        private List<string> _playlists { get; set; }

        /// <summary>
        /// List of songs which is bound on GUI
        /// </summary>
        public List<string> Songs 
        {
            get 
            {
                return _songs;
            } 
            private set 
            { 
                _songs = value;
                OnPropertyChanged(nameof(Songs));
            } 
        }
        /// <summary>
        /// List of playlists which is bound on GUI
        /// </summary>
        public List<string> Playlists 
        {
            get
            {
                return _playlists;
            } 
            private set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        /// <summary>
        /// Function is useless right now, but if you need, you can add some filters
        /// </summary>
        /// <param name="songs">List of songs</param>
        public void UpdateSongs(List<string> songs)
        {
            Songs = null;

            if (songs == null || songs.Count == 0) Songs = null;

            Songs = songs;
        }

        /// <summary>
        /// Function is useless right now, but if you need, you can add some filters
        /// </summary>
        /// <param name="playlists">List of playlists</param>
        public void UpdatePlaylist(List<string> playlists)
        {
            Playlists = null;

            if (playlists == null || playlists.Count == 0) Playlists = null;

            Playlists = playlists;
        }
    }
}
