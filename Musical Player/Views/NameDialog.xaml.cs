using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Musical_Player.Views
{
    /// <summary>
    /// Interaction logic for the NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        /// <summary>
        /// Opens namedialog
        /// </summary>
        public NameDialog()
        {
            // Initialize the components of the dialog window
            InitializeComponent();

            // Set the OK button to be disabled when the window is created
            OK.IsEnabled = false;
        }

        // Field to store the dialog result (whether OK button is clicked or not)
        private bool Result = false;

        /// <summary>
        /// Event handler for the OK button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Set the result to true and close the window
            Result = true;
            Close();
        }

        /// <summary>
        /// Event handler for the window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Set the DialogResult based on the result
            DialogResult = Result;
        }

        /// <summary>
        /// Event handler for the text change event in the input field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the text field is empty or null
            if (InputTextbox.Text.Length == 0 || string.IsNullOrWhiteSpace(InputTextbox.Text) || InputTextbox.Text.StartsWith(" "))
            {
                // If empty, disable the OK button
                OK.IsEnabled = false;
            }
            else
            {
                // Check for invalid characters in the input
                if (InputTextbox.Text.Contains("<")
                    || InputTextbox.Text.Contains(">")
                    || InputTextbox.Text.Contains("/")
                    || InputTextbox.Text.Contains(@"\")
                    || InputTextbox.Text.Contains(":")
                    || InputTextbox.Text.Contains("?")
                    || InputTextbox.Text.Contains("*")
                    || InputTextbox.Text.Contains(":")
                    || InputTextbox.Text.Contains("|")
                    || InputTextbox.Text.Contains('"'))
                {
                    OK.IsEnabled = false;
                    Tiplabel.Content = @"Name can't contain <>/\*|?:";
                }

                // Check for the maximum length of the input
                if (InputTextbox.Text.Length > 34)
                {
                    OK.IsEnabled = false;
                    Tiplabel.Content = @"Max 35 symbols!";
                }
                else
                {
                    // If there is text, enable the OK button and update the tip label
                    Tiplabel.Content = "Input new playlist name";
                    OK.IsEnabled = true;
                }
            }
        }
    }
}
