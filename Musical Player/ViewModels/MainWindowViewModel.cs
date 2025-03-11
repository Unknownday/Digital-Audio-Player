using Musical_Player.Contollers;
using Musical_Player.Files_management;
using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Musical_Player.ViewModels
{
    /// <summary>
    /// The viewmodel for 
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Variables & Setters

        /// <summary>
        /// Event which is called when choosen propretry has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private static MainWindowViewModel instance;
        private List<string> _songs { get; set; }
        private List<string> _playlists { get; set; }

        #region Strings

        private string songNameText { get; set; }
        private string playlistsTagText { get; set; }
        private string songsTagText { get; set; }
        private string repeatText { get; set; }
        private string dragTipText { get; set; }
        private string shuffleBtnText { get; set; }

        #endregion

        private int _selectedSong;

        private int _selectedPlaylist = 0;

        #region Brushes

        public ImageSource MuteBtnBrush { get; set; }
        public ImageSource PlayBtnBrush { get; set; }
        public ImageSource PauseBtnBrush { get; set; }
        public ImageSource SettingsBtnBrush { get; set; }
        public ImageSource InformationBtnBrush { get; set; }
        public ImageSource RenameBtnBrush { get; set; }
        public ImageSource ReloadBtnBrush { get; set; }
        public ImageSource RemovetnBrush { get; set; }
        public ImageSource BackwardBtnBrush { get; set; }
        public ImageSource ForwardBtnBrush { get; set; }
        public ImageSource AddBtnBrush { get; set; }
        public ImageSource UpBtnBrush { get; set; }
        public ImageSource DownBtnBrush { get; set; }
        public ImageSource ReplayBtnBrush { get; set; }

        #endregion

        #region Commands

        public ICommand AddSongBtnCommand { get; private set; }
        public ICommand BackwardBtnCommand { get; private set; }
        public ICommand CreatePlaylistBtnCommand { get; private set; }
        public ICommand DeletePlaylistBtnCommand { get; private set; }
        public ICommand DeleteSongBtnCommand { get; private set; }
        public ICommand DurationBarValueChangedCommand { get; private set; }
        public ICommand ForwardBtnCommand { get; private set; }
        public ICommand InfoBtnCommand { get; private set; }
        public ICommand MoveDownBtnCommand { get; private set; }
        public ICommand MoveUpBtnCommand { get; private set; }
        public ICommand MuteBtnCommand { get; private set; }
        public ICommand PauseBtnCommand { get; private set; }
        public ICommand PlayBtnCommand { get; private set; }
        public ICommand ReloadSongsCommand { get; private set; }
        public ICommand RenameBtnCommand { get; private set; }
        public ICommand ReplayBtnCommand { get; private set; }
        public ICommand SettingsBtnCommand { get; private set; }
        public ICommand ShuffleBtnCommand { get; private set; }
        public ICommand SongListSelectionChangedCommand { get; private set; }
        public ICommand PlaylistsListSelectionChangedCommand { get; private set; }
        public ICommand VolumeBarValueChangedCommand { get; private set; }
        public ICommand VolumeBarDeltaCommand { get; private set; }
        public ICommand DurationBarDeltaCommand { get; private set; }
        
        #endregion

        public SolidColorBrush ControllsColor { get; private set; }


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
            InitializeColors();
            InitializeIcons();
            InitializeCommands();
            InitializeTexts(model);

            

            PlaylistController.Instance.LoadPlaylists();
            Playlists = PlaylistController.Instance.GetPlaylistsHeaders();
           
            
            PlaylistController.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => Songs = PlaylistController.Instance.CurrentPlaylist == null ? null : PlaylistController.Instance.CurrentPlaylist.Songs.Select(song => FileManager.NameFilter(song.Name)).ToList();
            if (Playlists.Count > 0) PlaylistController.Instance.SelectPlaylist(Playlists[0]);
        }


        /// <summary>
        /// Function that inintialize colors of foreground
        /// </summary>
        private void InitializeColors()
        {
            var color = System.Drawing.Color.FromName(ThemeManager.Theme);
            ControllsColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
        }

        /// <summary>
        /// Function that loads brushes for buttons background
        /// </summary>
        /// <exception cref="InvalidOperationException">In case if icons weren't loaded.</exception>
        private void InitializeIcons()
        {
            if (ThemeManager.Icons == null)
            {
                throw new InvalidOperationException("ThemeManager is not initialized.");
            }

            var iconNames = new Dictionary<string, string>
            {
                { "MuteBtnBrush", "mute" },
                { "PlayBtnBrush", "play" },
                { "PauseBtnBrush", "pause" },
                { "SettingsBtnBrush", "settings" },
                { "InformationBtnBrush", "info" },
                { "RenameBtnBrush", "rename" },
                { "ReloadBtnBrush", "repeat" },
                { "RemovetnBrush", "delete" },
                { "BackwardBtnBrush", "backward" },
                { "ForwardBtnBrush", "forward" },
                { "AddBtnBrush", "add" },
                { "UpBtnBrush", "moveUp" },
                { "DownBtnBrush", "moveDown" },
                { "ReplayBtnBrush", "repeat" }
            };

            foreach (var icon in iconNames)
            {
                var property = GetType().GetProperty(icon.Key);
                if (property != null)
                {
                    property.SetValue(this, ThemeManager.Icons[icon.Value]?.ImageSource);
                }
            }
        }

        /// <summary>
        /// Binding function to the vars of type 'ICommand' to be binded in MainWindowView
        /// </summary>
        private void InitializeCommands()
        {
            var commandMethods = new Dictionary<string, Action>
            {
                { "AddSongBtnCommand", ExecuteAddSongBtn },
                { "BackwardBtnCommand", ExecuteBackwardBtn },
                { "CreatePlaylistBtnCommand", ExecuteCreatePlaylistBtn },
                { "DeletePlaylistBtnCommand", ExecuteDeletePlaylistBtn },
                { "DeleteSongBtnCommand", ExecuteDeleteSongBtn },
                { "DurationBarValueChangedCommand", ExecuteDurationBarValueChanged },
                { "ForwardBtnCommand", ExecuteForwardBtn },
                { "InfoBtnCommand", ExecuteInfoBtn },
                { "MoveDownBtnCommand", ExecuteMoveDownBtn },
                { "MoveUpBtnCommand", ExecuteMoveUpBtn },
                { "MuteBtnCommand", ExecuteMuteBtn },
                { "PauseBtnCommand", ExecutePauseBtn },
                { "PlayBtnCommand", ExecutePlayBtn },
                { "ReloadSongsCommand", ExecuteReloadSongs },
                { "RenameBtnCommand", ExecuteRenameBtn },
                { "ReplayBtnCommand", ExecuteReplayBtn },
                { "SettingsBtnCommand", ExecuteSettingsBtn },
                { "ShuffleBtnCommand", ExecuteShuffleBtn },
                { "VolumeBarValueChangedCommand", ExecuteVolumeBarValueChanged },
                { "VolumeBarDeltaCommand", ExecuteVolumeBarDelta },
                { "DurationBarDeltaCommand", ExecuteDurationBarDelta },
                { "PlaylistsListSelectionChangedCommand", ExecutePlaylistsListSelectionChangedCommand },
                { "SongListSelectionChangedCommand", ExecuteSongListSelectionChangedCommand}
            };

            foreach (var command in commandMethods)
            {
                var property = GetType().GetProperty(command.Key);
                if (property != null)
                {
                    property.SetValue(this, new RelayCommand(command.Value));
                }
            }
        }

        /// <summary>
        /// Init texts of all the labels on the frontend
        /// </summary>
        /// <param name="model">Language model instance</param>
        /// <exception cref="ArgumentNullException">In case if Language model is not provided</exception>
        private void InitializeTexts(LanguageModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ShuffleBtnText = model.ShuffleBtnText;
            SongNameText = model.SongNameText;
            PlaylistsTagText = model.PlaylistsTagText;
            SongsTagText = model.SongsTagText;
            RepeatText = model.RepeatIndicator;
            DragTipText = model.DragTipText;
        }

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

        
        public int SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                if (_selectedSong != value)
                {
                    _selectedSong = value;
                    OnPropertyChanged(nameof(SelectedSong));
                }
            }
        }

        public int SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        #endregion

        #region Commands

        private void ExecuteAddSongBtn()
        {

            PlaylistController.Instance.AddSongToPlaylist();

            PlaylistController.Instance.LoadPlaylists();

            Playlists = PlaylistController.Instance.GetPlaylistsHeaders();

            PlaylistController.Instance.SelectPlaylist(Playlists[SelectedPlaylist]);
        }

        private void ExecuteBackwardBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteCreatePlaylistBtn()
        {
            PlaylistController.Instance.CreatePlaylist();

            PlaylistController.Instance.LoadPlaylists();

            Playlists = PlaylistController.Instance.GetPlaylistsHeaders();

        }

        private void ExecuteDeletePlaylistBtn()
        {
            PlaylistController.Instance.DeletePlaylist();

            PlaylistController.Instance.LoadPlaylists();

            Playlists = PlaylistController.Instance.GetPlaylistsHeaders();
        }

        private void ExecuteDeleteSongBtn()
        {
            PlayerController.StopRecording();

            PlaylistController.Instance.DeleteSong(_selectedSong);

            PlaylistController.Instance.LoadPlaylists();

            Playlists = PlaylistController.Instance.GetPlaylistsHeaders();

            PlaylistController.Instance.SelectPlaylist(Playlists[SelectedPlaylist]);
        }

        private void ExecuteDurationBarValueChanged()
        {
            throw new NotImplementedException();
        }

        private void ExecuteForwardBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteInfoBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteMoveDownBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteMoveUpBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteMuteBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecutePauseBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecutePlayBtn()
        {
            throw new NotImplementedException();
        }


        private void ExecuteReloadSongs()
        {
            throw new NotImplementedException();
        }

        private void ExecuteRenameBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteReplayBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteSettingsBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteShuffleBtn()
        {
            throw new NotImplementedException();
        }

        private void ExecuteVolumeBarValueChanged()
        {
            throw new NotImplementedException();
        }

        private void ExecuteDurationBarDelta()
        {
            throw new NotImplementedException();
        }

        private void ExecuteVolumeBarDelta()
        {
            throw new NotImplementedException();
        }
        private void ExecuteSongListSelectionChangedCommand()
        {
           
        }

        private void ExecutePlaylistsListSelectionChangedCommand()
        {
            PlaylistController.Instance.SelectPlaylist(Playlists[SelectedPlaylist >= Playlists.Count ? 0 : _selectedPlaylist]);
            
        }

        #endregion

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

    public class RelayCommand : ICommand  
    {  
        private readonly Action _execute;  
        private readonly Func<bool> _canExecute;  

        public RelayCommand(Action execute, Func<bool> canExecute = null)  
        {  
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));  
            _canExecute = canExecute;  
        }  

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();  

        public event EventHandler CanExecuteChanged  
        {  
            add { CommandManager.RequerySuggested += value; }  
            remove { CommandManager.RequerySuggested -= value; }  
        }  

        public void Execute(object parameter) => _execute();  
    }
}
