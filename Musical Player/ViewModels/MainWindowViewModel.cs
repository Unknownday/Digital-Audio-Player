using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private static MainWindowViewModel instance;
        private List<string> _songs { get; set; }
        private List<string> _playlists { get; set; }
        private string songNameText { get; set; }
        private string playlistsTagText { get; set; }
        private string songsTagText { get; set; }
        private string repeatText { get; set; }
        private string dragTipText { get; set; }
        private string shuffleBtnText { get; set; }
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
        public ICommand AddSongBtnCommand { get; }
        public ICommand BackwardBtnCommand { get; }
        public ICommand CreatePlaylistBtnCommand { get; }
        public ICommand DeletePlaylistBtnCommand { get; }
        public ICommand DeleteSongBtnCommand { get; }
        public ICommand DurationBarValueChangedCommand { get; }
        public ICommand ForwardBtnCommand { get; }
        public ICommand InfoBtnCommand { get; }
        public ICommand MoveDownBtnCommand { get; }
        public ICommand MoveUpBtnCommand { get; }
        public ICommand MuteBtnCommand { get; }
        public ICommand PauseBtnCommand { get; }
        public ICommand PlayBtnCommand { get; }
        public ICommand PlaylistSelectionChangedCommand { get; }
        public ICommand ReloadSongsCommand { get; }
        public ICommand RemovePlaylistBtnCommand { get; }
        public ICommand RemoveSongBtnCommand { get; }
        public ICommand RenameBtnCommand { get; }
        public ICommand ReplayBtnCommand { get; }
        public ICommand SettingsBtnCommand { get; }
        public ICommand ShuffleBtnCommand { get; }
        public ICommand SongListSelectionChangedCommand { get; }
        public ICommand VolumeBarValueChangedCommand { get; }
        public ICommand VolumeBarDeltaCommand { get; }
        public ICommand DurationBarDeltaCommand { get; }
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
            var color = System.Drawing.Color.FromName(ThemeManager.Theme);
            ControllsColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));

            MuteBtnBrush = ThemeManager.Icons["mute"].ImageSource;
            PlayBtnBrush = ThemeManager.Icons["play"].ImageSource;
            PauseBtnBrush = ThemeManager.Icons["pause"].ImageSource;
            SettingsBtnBrush = ThemeManager.Icons["settings"].ImageSource;
            InformationBtnBrush = ThemeManager.Icons["info"].ImageSource;
            RenameBtnBrush = ThemeManager.Icons["rename"].ImageSource;
            ReloadBtnBrush = ThemeManager.Icons["repeat"].ImageSource;
            RemovetnBrush = ThemeManager.Icons["delete"].ImageSource;
            BackwardBtnBrush = ThemeManager.Icons["backward"].ImageSource;
            ForwardBtnBrush = ThemeManager.Icons["forward"].ImageSource;
            AddBtnBrush = ThemeManager.Icons["add"].ImageSource;
            UpBtnBrush = ThemeManager.Icons["moveUp"].ImageSource;
            DownBtnBrush = ThemeManager.Icons["moveDown"].ImageSource;
            ReplayBtnBrush = ThemeManager.Icons["repeat"].ImageSource;

            AddSongBtnCommand = new RelayCommand(ExecuteAddSongBtn);
            BackwardBtnCommand = new RelayCommand(ExecuteBackwardBtn);
            CreatePlaylistBtnCommand = new RelayCommand(ExecuteCreatePlaylistBtn);
            DeletePlaylistBtnCommand = new RelayCommand(ExecuteDeletePlaylistBtn);
            DeleteSongBtnCommand = new RelayCommand(ExecuteDeleteSongBtn);
            DurationBarValueChangedCommand = new RelayCommand(ExecuteDurationBarValueChanged);
            ForwardBtnCommand = new RelayCommand(ExecuteForwardBtn);
            InfoBtnCommand = new RelayCommand(ExecuteInfoBtn);
            MoveDownBtnCommand = new RelayCommand(ExecuteMoveDownBtn);
            MoveUpBtnCommand = new RelayCommand(ExecuteMoveUpBtn);
            MuteBtnCommand = new RelayCommand(ExecuteMuteBtn);
            PauseBtnCommand = new RelayCommand(ExecutePauseBtn);
            PlayBtnCommand = new RelayCommand(ExecutePlayBtn);
            PlaylistSelectionChangedCommand = new RelayCommand(ExecutePlaylistSelectionChanged);
            ReloadSongsCommand = new RelayCommand(ExecuteReloadSongs);
            RemovePlaylistBtnCommand = new RelayCommand(ExecuteRemovePlaylistBtn);
            RemoveSongBtnCommand = new RelayCommand(ExecuteRemoveSongBtn);
            RenameBtnCommand = new RelayCommand(ExecuteRenameBtn);
            ReplayBtnCommand = new RelayCommand(ExecuteReplayBtn);
            SettingsBtnCommand = new RelayCommand(ExecuteSettingsBtn);
            ShuffleBtnCommand = new RelayCommand(ExecuteShuffleBtn);
            SongListSelectionChangedCommand = new RelayCommand(ExecuteSongListSelectionChanged);
            VolumeBarValueChangedCommand = new RelayCommand(ExecuteVolumeBarValueChanged);
            VolumeBarDeltaCommand = new RelayCommand(ExecuteVolumeBarDelta);
            DurationBarDeltaCommand = new RelayCommand(ExecuteDurationBarDelta);

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

        #endregion

        #region Commands

        private void ExecuteAddSongBtn()
        {

        }

        private void ExecuteBackwardBtn()
        {

        }

        private void ExecuteCreatePlaylistBtn()
        {

        }

        private void ExecuteDeletePlaylistBtn()
        {

        }

        private void ExecuteDeleteSongBtn()
        {

        }

        private void ExecuteDurationBarValueChanged()
        {

        }

        private void ExecuteForwardBtn()
        {

        }

        private void ExecuteInfoBtn()
        {

        }

        private void ExecuteMoveDownBtn()
        {

        }

        private void ExecuteMoveUpBtn()
        {

        }

        private void ExecuteMuteBtn()
        {

        }

        private void ExecutePauseBtn()
        {

        }

        private void ExecutePlayBtn()
        {

        }

        private void ExecutePlaylistSelectionChanged()
        {

        }

        private void ExecuteReloadSongs()
        {

        }

        private void ExecuteRemovePlaylistBtn()
        {

        }

        private void ExecuteRemoveSongBtn()
        {

        }

        private void ExecuteRenameBtn()
        {

        }

        private void ExecuteReplayBtn()
        {

        }

        private void ExecuteSettingsBtn()
        {

        }

        private void ExecuteShuffleBtn()
        {

        }

        private void ExecuteSongListSelectionChanged()
        {

        }

        private void ExecuteVolumeBarValueChanged()
        {

        }

        private void ExecuteDurationBarDelta()
        {

        }

        private void ExecuteVolumeBarDelta()
        {

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
