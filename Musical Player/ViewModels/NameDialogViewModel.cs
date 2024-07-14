using Musical_Player.Global;
using Musical_Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.ViewModels
{
    /// <summary>
    /// View model of name dialog
    /// </summary>
    public class NameDialogViewModel
    {
        private static InfoWindowViewModel instance;
        private string textBoxTag { get; set; }
        private string submitBtnText { get; set; }

        /// <summary>
        /// Instance of this ViewModel
        /// </summary>
        public static InfoWindowViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InfoWindowViewModel(Config.LanguageModel);
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
        /// Constructor of this ViewModel
        /// </summary>
        /// <param name="model">Language model</param>
        public NameDialogViewModel(LanguageModel model)
        {
            this.TextBoxTag = model.TextBoxTag;
            this.SubmitBtnText = model.SubmitBtnText;
        }

        /// <summary>
        /// Content for inputbox tag
        /// </summary>
        public string TextBoxTag 
        { 
            get 
            {
                return textBoxTag;
            } 
            set
            {
                textBoxTag = value;
                OnPropertyChanged(TextBoxTag);
            }
        }

        /// <summary>
        /// Content for submit button
        /// </summary>
        public string SubmitBtnText
        {
            get
            {
                return submitBtnText;
            }
            set
            {
                submitBtnText = value;
                OnPropertyChanged(SubmitBtnText);
            }
        }
    }
}
