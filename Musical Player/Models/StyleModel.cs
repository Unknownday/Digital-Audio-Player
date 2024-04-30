using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Musical_Player.Models
{
    /// <summary>
    /// Style model to save model
    /// </summary>
    public static class StyleModel
    {
        /// <summary>
        /// Text's color
        /// </summary>
        public static SolidColorBrush ForegroundColor { get; set; } = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Button's color
        /// </summary>
        public static SolidColorBrush ButtonColors { get; set; } = new SolidColorBrush(Colors.White);
    }
}
