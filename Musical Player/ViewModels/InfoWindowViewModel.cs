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
    public class InfoWindowViewModel
    {
        private static InfoWindowViewModel instance;

        private string label1Text { get; set; } 
        private string label2Text { get; set; }
        private string closeBtnText { get; set; }
        private string buildVerText { get; set; }

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
        /// Constructor of InfoWindow view model
        /// </summary>
        /// <param name="model">Language model</param>
        public InfoWindowViewModel(LanguageModel model)
        {
            this.Label1Text = model.Label1Text;
            this.Label2Text = model.Label2Text;
            this.CloseBtnText = model.CloseBtnText;
            this.BuildVerText = model.BuildVerText;
        }

        /// <summary>
        /// Content for first label in the inforamtion window
        /// </summary>
        public string Label1Text
        {
            get
            {
                return label1Text;
            }
            set
            {
                label1Text = value;
                OnPropertyChanged(Label1Text);
            }
        }
        /// <summary>
        /// Content for the label in the information window 
        /// </summary>
        public string Label2Text
        {
            get
            {
                return label2Text;
            }
            set
            {
                label2Text = value;
                OnPropertyChanged(Label2Text);
            }
        }
        /// <summary>
        /// Content for the button which closing inforamtion window
        /// </summary>
        public string CloseBtnText
        {
            get
            {
                return closeBtnText;
            }
            set
            {
                closeBtnText = value;
                OnPropertyChanged(CloseBtnText);
            }
        }
        /// <summary>
        /// Contnet for label with build
        /// </summary>
        public string BuildVerText
        {
            get
            {
                return buildVerText;
            }
            set
            {
                buildVerText = value;
                OnPropertyChanged(BuildVerText);
            }
        }
    }
}
