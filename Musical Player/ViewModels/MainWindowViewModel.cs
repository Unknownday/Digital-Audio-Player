using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Documents;
using System.Windows.Media;

namespace Musical_Player.ViewModels
{
    /// <summary>
    /// The viewmodel for 
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private static MainWindowViewModel instance;
        private List<string> _songs { get; set; }
        private List<string> _playlists { get; set; }
        private string songNameText { get; set; }
        private string playlistsTagText { get; set; }
        private string songsTagText { get; set; }
        private string repeatText { get; set; }
        private string dragTipText { get; set; }
        private string shuffleBtnText { get; set; }

        /// <summary>
        /// Instance of this ViewModel
        /// </summary>
        public static MainWindowViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainWindowViewModel(Config.LanguageModel);
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor of main window view model
        /// </summary>
        public MainWindowViewModel(LanguageModel model)
        {
            this.ShuffleBtnText = model.ShuffleBtnText;
            this.SongNameText = model.SongNameText;
            this.PlaylistsTagText = model.PlaylistsTagText;
            this.SongsTagText = model.SongsTagText;
            this.RepeatText = model.RepeatIndicator;
            this.DragTipText = model.DragTipText;
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
        /// Content for button which shuffling songs
        /// </summary>
        public string ShuffleBtnText
        {
            get
            {
                return shuffleBtnText;
            }
            set
            {
                shuffleBtnText = value;
                OnPropertyChanged(ShuffleBtnText);
            }
        }

        /// <summary>
        /// Content for song tag until song will choosed
        /// </summary>
        public string SongNameText
        {
            get
            {
                return songNameText;
            }
            set
            {
                songNameText = value;
                OnPropertyChanged(SongNameText);
            }
        }
        
        /// <summary>
        /// Content for Tag of ListBox of playlists 
        /// </summary>
        public string PlaylistsTagText
        {
            get
            {
                return playlistsTagText;
            }
            set
            {
                playlistsTagText = value;
                OnPropertyChanged(PlaylistsTagText);
            }
        }

        /// <summary>
        /// Content for Tag of ListBox of songs 
        /// </summary>
        public string SongsTagText
        {
            get
            {
                return songsTagText;
            }
            set
            {
                songsTagText = value;
                OnPropertyChanged(SongsTagText);
            }
        }

        /// <summary>
        /// Content for repeat indicator
        /// </summary>
        public string RepeatText
        {
            get
            {
                return repeatText;
            }
            set
            {
                repeatText = value;
                OnPropertyChanged(RepeatText);
            }
        }
        
        /// <summary>
        /// Content for helping label while dragging file
        /// </summary>
        public string DragTipText
        {
            get
            {
                return dragTipText;
            }
            set
            {
                dragTipText = value;
                OnPropertyChanged(DragTipText);
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
