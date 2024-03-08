using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musical_Player.Global
{
    /// <summary>
    /// Перечисления используемые в проекте
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Состояния плейера
        /// </summary>
        public enum PlayerStates
        {
            InProgress,
            OnPause,
            Idle
        }

        /// <summary>
        /// Сторона для перемотки
        /// </summary>
        public enum MoveDirections
        {
            Up,
            Down
        }        
    }
}
