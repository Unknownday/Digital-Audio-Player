using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.ViewModels
{
    public class SettingWindowViewModel : INotifyPropertyChanged
    {
        private static SettingWindowViewModel instance;
        private string autoSwitchBtnText { get; set; }
        private string changeDirectoryBtnText { get; set; }
        private string changeBgBtnText { get; set; }
        private string switchThemeTag { get; set; }
        private string themeToggler { get; set; }

        /// <summary>
        /// Instance of this ViewModel.
        /// </summary>
        public static SettingWindowViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingWindowViewModel(Config.LanguageModel);
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

        /// <summary>
        /// Constructor of SettingWindow view model
        /// </summary>
        /// <param name="model"></param>
        public SettingWindowViewModel(LanguageModel model)
        {
            this.AutoSwitchBtnText = model.AutoSwitchBtnText;
            this.ChangeDirectoryBtnText = model.ChangeDirectoryBtnText;
            this.ChangeBgBtnText = model.ChangeBgBtnText;
            this.SwitchThemeTag = model.SwitchThemeTag;
            this.ThemeToggler = model.ThemeToggler;
        }

        /// <summary>
        /// Content for autoswitch button
        /// </summary>
        public string AutoSwitchBtnText
        {
            get
            {
                return autoSwitchBtnText;
            }
            set
            {
                autoSwitchBtnText = value;
                OnPropertyChanged(AutoSwitchBtnText);
            }
        }
        /// <summary>
        /// Content for change directory button
        /// </summary>
        public string ChangeDirectoryBtnText
        {
            get
            {
                return changeDirectoryBtnText;
            }
            set
            {
                changeDirectoryBtnText = value;
                OnPropertyChanged(ChangeDirectoryBtnText);
            }
        }
        /// <summary>
        /// Content for button of background choosing
        /// </summary>
        public string ChangeBgBtnText 
        {
            get
            {
                return changeBgBtnText;
            }
            set
            {
                changeBgBtnText = value;
                OnPropertyChanged(ChangeBgBtnText);
            }
        }
        /// <summary>
        /// Content for the switch theme tag
        /// </summary>
        public string SwitchThemeTag
        {
            get
            {
                return switchThemeTag;
            }
            set
            {
                switchThemeTag = value;
                OnPropertyChanged(SwitchThemeTag);
            }
        }
        /// <summary>
        /// Content for the tag of theme toggler
        /// </summary>
        public string ThemeToggler
        {
            get
            {
                return themeToggler;
            }
            set
            {
                themeToggler = value;
                OnPropertyChanged(ThemeToggler);
            }
        }
    }
}
