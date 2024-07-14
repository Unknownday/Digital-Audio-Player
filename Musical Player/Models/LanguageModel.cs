using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.Models
{
    /// <summary>
    /// The model used to keep elements text.
    /// </summary>
    public class LanguageModel
    {
        #region MainWindow
        /// <summary>
        /// Content for button which shuffling songs
        /// </summary>
        public string ShuffleBtnText { get; set; }
        /// <summary>
        /// Content for song tag until song will choosed
        /// </summary>
        public string SongNameText { get; set; }
        /// <summary>
        /// Content for Tag of ListBox of playlists 
        /// </summary>
        public string PlaylistsTagText { get; set; }
        /// <summary>
        /// Content for Tag of ListBox of songs 
        /// </summary>
        public string SongsTagText { get; set; }

        public string RepeatIndicator { get; set; }

        /// <summary>
        /// Content for helping label while dragging file
        /// </summary>
        public string DragTipText { get; set; }

        #endregion

        #region InfoWindow
        /// <summary>
        /// Content for first label in the inforamtion window
        /// </summary>
        public string Label1Text { get; set; }
        /// <summary>
        /// Content for the label in the information window 
        /// </summary>
        public string Label2Text { get; set; }
        /// <summary>
        /// Content for the button which closing inforamtion window
        /// </summary>
        public string CloseBtnText { get; set; }
        /// <summary>
        /// Contnet for label with build
        /// </summary>
        public string BuildVerText { get; set; }

        #endregion

        #region NameDialogWindow
        /// <summary>
        /// Content for inputbox tag
        /// </summary>
        public string TextBoxTag { get; set; }
        /// <summary>
        /// Content for submit button
        /// </summary>
        public string SubmitBtnText { get; set; }

        #endregion

        #region SettingsWindow 
        /// <summary>
        /// Content for autoswitch button
        /// </summary>
        public string AutoSwitchBtnText { get; set; }
        /// <summary>
        /// Content for change directory button
        /// </summary>
        public string ChangeDirectoryBtnText { get; set; }
        /// <summary>
        /// Content for button of background choosing
        /// </summary>
        public string ChangeBgBtnText { get; set; }
        /// <summary>
        /// Content for the switch theme tag
        /// </summary>
        public string SwitchThemeTag { get; set; }
        /// <summary>
        /// Content for the tag of theme toggler
        /// </summary>
        public string ThemeToggler { get; set; }

        #endregion

    }
}
