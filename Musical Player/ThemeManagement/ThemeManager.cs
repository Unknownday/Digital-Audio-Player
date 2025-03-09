using Musical_Player.Files_management;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Musical_Player.Models
{
    public class ThemeManager
    {
        private static ThemeManager _instance;

        public static Dictionary<string, ImageBrush> Icons = new Dictionary<string, ImageBrush>();

        public static string Theme = "Black";

        private ThemeManager()
        {
            try
            {
                var brushes = FileManager.CreateIconsBitmap(Theme);
                MessageBox.Show(brushes.Count.ToString());
                Icons.Add("play", brushes[0]);
                Icons.Add("pause", brushes[1]);
                Icons.Add("forward", brushes[2]);
                Icons.Add("backward", brushes[3]);
                Icons.Add("delete", brushes[4]);
                Icons.Add("useless", brushes[5]);
                Icons.Add("info", brushes[6]);
                Icons.Add("add", brushes[7]);
                Icons.Add("mute", brushes[8]);
                Icons.Add("moveUp", brushes[9]);
                Icons.Add("moveDown", brushes[10]);
                Icons.Add("settings", brushes[11]);
                Icons.Add("repeat", brushes[12]);
                Icons.Add("rename", brushes[13]);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error ocurred! Stopping programm! If same happens after restart - please contact me unknownday.game@gmail.com", "Digital Audio Player Error Handler", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.Message);
            }
        }

        public void Update()
        {
            _instance = new ThemeManager();
        }
        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThemeManager();
                }
                

                return _instance;
            }
        }
    }
}
